using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.Enemies;

//Asser, Oscar
public abstract class Enemy : Character
{
    #region Properties
    private Grid grid;
    private Astar astar;
    private GameObject playerGo; 
    public Player Player;
    private SpriteRenderer weaponSpriteRenderer;

    public List<GameObject> Path { get; private set; }
    private Vector2 nextTarget;
    public Point TargetPoint;
    private readonly float threshold = 10f;

    private bool hasBeenAwoken;

    #endregion Properties
    public Enemy(GameObject gameObject) : base(gameObject)
    {
        Speed = 250;
    }
    public override void Awake()
    {
        base.Awake();

        astar = new Astar();

        if (WeaponGo != null)
        {
            weaponSpriteRenderer = WeaponGo.GetComponent<SpriteRenderer>();  
        }

        Collider.SetCollisionBox(15, 27, new Vector2(0, 15));
    }

    public void SetStartPosition(GameObject playerGo, Point gridPos)
    {
        this.playerGo = playerGo;
        Player = playerGo.GetComponent<Player>();

        TargetPoint = playerGo.Transform.GridPosition;
        grid = GridManager.Instance.CurrentGrid;
        GameObject.Transform.GridPosition = gridPos;
    }

    private List<Enemy> enemyList = new();
    public void SetStartEnemyRefs(List<Enemy> enemies)
    {
        enemyList = enemies.Where(enemy => enemy != this).ToList();
    }

    public override void Start()
    {
        astar.Start(this, enemyList);
        SpriteRenderer.SetLayerDepth(LayerDepth.EnemyUnder);

        SetState(CharacterState.Idle);

        // Sets start position
        GameObject currentCellGo = grid.GetCellGameObjectFromPoint(GameObject.Transform.GridPosition);
        GameObject.Transform.Position = currentCellGo.Transform.Position;
        Cell cell = currentCellGo.GetComponent<Cell>();
        CollisionNr = cell.CollisionNr;

        Weapon.MoveWeapon();

        if (Player.CollisionNr == CollisionNr) SetPath(); // We know that the player the targetPoint has been set
    }

    public override void Update(GameTime gameTime)
    {
        CheckLayerDepth(); // Make sure the enemy is drawn correctly.

        //To make a new path towards the player, if they have moved.
        PlayerMovedInRoom();

        switch (State)
        {
            case CharacterState.Idle:
                break;

            case CharacterState.Moving:
                UpdatePathing();
                break;

            case CharacterState.Attacking:

                // Do nothing if the player has died.
                if (Player.State == CharacterState.Dead) return;

                // Update direction towards player insted of moving direction
                Direction = Vector2.Normalize(playerGo.Transform.Position - GameObject.Transform.Position);
                Weapon.MoveWeapon();
                UpdateDirection();
                Attack();
                break;

            case CharacterState.Dead:
                break;
        }
    }

    private void PlayerMovedInRoom()
    {
        if (Player.CollisionNr != CollisionNr && !hasBeenAwoken || State == CharacterState.Dead) return; // Cant move if the player isnt in the same room.

        Point playerPos = playerGo.Transform.GridPosition;
        int precise = 3; 
        // If X is more that z cells away, it should start a new target. The same with Y
        if (Math.Abs(playerPos.X - TargetPoint.X) >= precise ||
            Math.Abs(playerPos.Y - TargetPoint.Y) >= precise)
        {
            TargetPoint = playerGo.Transform.GridPosition;
            hasBeenAwoken = true;
            SetPath();
        }
    }

    private void CheckLayerDepth()
    {
        if (GameObject.Transform.Position.Y < playerGo.Transform.Position.Y)
        {
            SpriteRenderer.SetLayerDepth(LayerDepth.EnemyUnder);

            if (weaponSpriteRenderer == null) return;
            weaponSpriteRenderer.SetLayerDepth(LayerDepth.EnemyUnderWeapon);
        }
        else
        {
            SpriteRenderer.SetLayerDepth(LayerDepth.EnemyOver);

            if (weaponSpriteRenderer == null) return;
            weaponSpriteRenderer.SetLayerDepth(LayerDepth.EnemyOverWeapon);
        }
    }

    #region PathFinding
    Random rnd = new();
    bool setPathRunning;
    private void SetPath()
    {
        ResetPathColor(); // For debugging

        Path = null; // We cant use the previous path
        // Create a new thread to find the path 
        Path = astar.FindPath(GameObject.Transform.GridPosition, TargetPoint);
        if (State == CharacterState.Dead) // Bug happened because this path got returned just as it died
        {
            setPathRunning = false;
            return;
        }
        if (Path != null && Path.Count > 0)
        {
            // If a new path is being set, set the next target to the enemy's current position
            SetState(CharacterState.Moving);
            if (GameObject.Transform.Position != Path[0].Transform.Position)
            {
                nextTarget = GameObject.Transform.Position;
            }
            else
            {
                SetNextTargetPos(Path[0]);
            }
        }
        //if (setPathRunning) return;

        //Thread thread = new(() =>
        //{
        //    setPathRunning = true;
        //    // Sleep for a little time, so the threads dont start at the same time
        //    Thread.Sleep(rnd.Next(1, 20)); 

        //    path = astar.FindPath(GameObject.Transform.GridPosition, targetPoint);
        //    if (State == CharacterState.Dead) // Bug happened because this path got returned just as it died
        //    {
        //        setPathRunning = false;
        //        return;
        //    }
        //    if (path != null && path.Count > 0)
        //    {
        //        // If a new path is being set, set the next target to the enemy's current position
        //        SetState(CharacterState.Moving);
        //        if (GameObject.Transform.Position != path[0].Transform.Position)
        //        {
        //            nextTarget = GameObject.Transform.Position;
        //        }
        //        else
        //        {
        //            SetNextTargetPos(path[0]);
        //        }
        //    }

        //    setPathRunning = false;
        //});
        //thread.IsBackground = true;
        //thread.Start();
    }

    private void UpdatePathing()
    {
        if (Path == null)
            return;
        Vector2 position = GameObject.Transform.Position;

        if (Vector2.Distance(position, nextTarget) < threshold)
        {
            if (Path.Count > 2)
            {
                GameObject.Transform.GridPosition = Path[0].Transform.GridPosition;
                UpdateRoomNr(Path[0]);
                ResetCellColor(Path[0]);
                Path.RemoveAt(0);
                SetNextTargetPos(Path[0]);
            }
            else if (Path.Count == 2) // Stops the path.
            {
                GameObject.Transform.GridPosition = Path[0].Transform.GridPosition;
                UpdateRoomNr(Path[0]);
                SetNextTargetPos(Path[0]);
                ResetCellColor(Path[0]);
                Path.RemoveAt(0);
            }
        }

        Direction = Vector2.Normalize(nextTarget - position);

        GameObject.Transform.Translate(Direction * Speed * GameWorld.DeltaTime);
        Weapon.MoveWeapon();

        if (Path.Count <= 3) // Attack before reaching the player, to put pressure on them
        {
            Direction = Vector2.Normalize(playerGo.Transform.Position - GameObject.Transform.Position);
            Attack();
        }

        if (Path.Count == 1 && Vector2.Distance(position, nextTarget) < threshold)
        {
            SetState(CharacterState.Attacking); // Close so would always attack
    
            ResetCellColor(Path[0]); // Debug

            Path = null;
        }

        UpdateDirection();
    }

    private void UpdateRoomNr(GameObject cellGo)
    {
        Cell cell = cellGo.GetComponent<Cell>();
        CollisionNr = cell.CollisionNr;
    }

    private void ResetPathColor()
    {
        if (Path != null)
        {
            foreach (GameObject cellGo in Path)
            {
                ResetCellColor(cellGo);
            }
        }
    }

    private void ResetCellColor(GameObject cellGo)
    {
        Cell cell = cellGo.GetComponent<Cell>();
        cell.ChangeCellWalkalbeType(cell.CellWalkableType); // Dont change the type, but makes it draw the color of the debug, to the correct color.
    }

    private void SetNextTargetPos(GameObject cellGo)
    {
        nextTarget = cellGo.Transform.Position + new Vector2(0, -Cell.dimension * Cell.Scale / 2);
    }

    #endregion PathFinding
}