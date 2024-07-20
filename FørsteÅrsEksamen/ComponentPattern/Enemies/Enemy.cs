using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

    private List<Enemy> enemyList = new();
    /// <summary>
    /// Astar path with a list of Cell GameObjects. Gets set in astar
    /// </summary>
    public List<GameObject> Path { get; private set; }
    private GameObject playerTarget;
    private Vector2 nextTarget;
    public Point TargetPoint;
    private readonly float thresholdToTargetCell = 10f; // For distance check
    /// <summary>
    /// The frequency of new paths
    /// </summary>
    public int CellPlayerMoveBeforeNewTarget = 3;       

    private bool hasBeenAwoken;

    private Random rnd = new();
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
        // Make this better or remove the couple of Get functions in the grid.
        playerTarget = GridManager.Instance.CurrentGrid.GetCellFromPoint(gridPos).GameObject; 
        grid = GridManager.Instance.CurrentGrid;
        GameObject.Transform.GridPosition = gridPos;
    }

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

        // If X is more that z cells away, it should start a new target. The same with Y
        if (Math.Abs(playerPos.X - TargetPoint.X) >= CellPlayerMoveBeforeNewTarget ||
            Math.Abs(playerPos.Y - TargetPoint.Y) >= CellPlayerMoveBeforeNewTarget)
        {
            TargetPoint = playerGo.Transform.GridPosition;
            hasBeenAwoken = true;
            SetPath();
        }
    }

    private void CheckLayerDepth()
    {
        // Offset for layerdepth, so the enemies are not figting for which is shown.
        float offSet = GameObject.Transform.GridPosition.Y / 10_000f; 
        if (GameObject.Transform.Position.Y < playerGo.Transform.Position.Y)
        {
            SpriteRenderer.SetLayerDepth(LayerDepth.EnemyUnder, offSet);

            if (weaponSpriteRenderer == null) return;
            weaponSpriteRenderer.SetLayerDepth(LayerDepth.EnemyUnderWeapon, offSet);
        }
        else
        {
            SpriteRenderer.SetLayerDepth(LayerDepth.EnemyOver, offSet);

            if (weaponSpriteRenderer == null) return;
            weaponSpriteRenderer.SetLayerDepth(LayerDepth.EnemyOverWeapon, offSet);
        }
    }

    #region PathFinding


    private void SetPath()
    {
        //ResetPathColor(); // For debugging

        Path = null; // We cant use the previous path
        // Create a new thread to find the path
        Path = astar.FindPath(GameObject.Transform.GridPosition, TargetPoint);
        // Bug happened because this path got returned just as it died
        if (State == CharacterState.Dead) return;

        // If there has been found no path. Maybe here it could check astar again?
        if (Path == null || Path.Count == 0) return;

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

    private int distanceStopFromTarget = 2; // The amount of distance the enemy has to the player

    public bool PointUnitsApart(Point first, Point second, int targetDistance)
    {
        //double distance = Math.Sqrt(Math.Pow(second.X - first.X, 2) + Math.Pow(second.Y - first.Y, 2));
        double distance = Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y);
        return distance >= targetDistance;
    }

    private void UpdatePathing()
    {
        if (Path == null)
            return;
        Vector2 position = GameObject.Transform.Position;

        if (Vector2.Distance(position, nextTarget) < thresholdToTargetCell)
        {
            bool enemyOnPath = false;
            foreach (Enemy enemy in enemyList)
            {
                if (Path.Count <= 1) break;

                // Checks the pos of the enemy, both if there already on the grid position or if they are moving
                // Make the bool true, to keep going on the path, so not to have multiple enemies in one spot.
                if (enemy.GameObject.Transform.GridPosition != Path[1].Transform.GridPosition ||
                    enemy.Path != null && enemy.Path.Count > 0 && enemy.Path[^1].Transform.GridPosition == Path[1].Transform.GridPosition)
                {
                    enemyOnPath = true;
                    break;
                }
            }

            if (Path.Count > distanceStopFromTarget || enemyOnPath) // If the second last in the path is the target, it should not stop inside the player
            {
                GameObject.Transform.GridPosition = Path[0].Transform.GridPosition;
                UpdateRoomNr(Path[0]);
                Path.RemoveAt(0);
                SetNextTargetPos(Path[0]);
                
            }
            else if (Path.Count == distanceStopFromTarget) // Stops the path.
            {
                GameObject.Transform.GridPosition = Path[0].Transform.GridPosition;
                UpdateRoomNr(Path[0]);
                SetNextTargetPos(Path[0]);
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

        if (Path.Count == 1 && Vector2.Distance(position, nextTarget) < thresholdToTargetCell)
        {
            SetState(CharacterState.Attacking); // Close so would always attack
            Path = null;
        }

        UpdateDirection();
    }

    private void UpdateRoomNr(GameObject cellGo)
    {
        Cell cell = cellGo.GetComponent<Cell>();
        CollisionNr = cell.CollisionNr;
    }

    private void SetNextTargetPos(GameObject cellGo)
    {
        nextTarget = cellGo.Transform.Position + new Vector2(0, -Cell.dimension * Cell.Scale / 2);
    }
    #endregion PathFinding
}