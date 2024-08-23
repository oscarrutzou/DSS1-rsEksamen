using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.Other;
using LiteDB;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;

namespace DoctorsDungeon.ComponentPattern.Enemies;

//Asser, Oscar
public abstract class Enemy : Character
{
    #region Properties


    public Astar Astar { get; private set; }
    protected GameObject PlayerGo;
    public Player Player;
    private SpriteRenderer _weaponSpriteRenderer;

    private List<Enemy> _enemyListRefs = new();
    /// <summary>
    /// Astar path with a list of Cell GameObjects. Gets set in astar
    /// </summary>
    public List<GameObject> Path { get; private set; }
    private Vector2 _nextTarget;
    public Point TargetPoint;
    private readonly float _thresholdToTargetCell = 10f; // For distance check
    /// <summary>
    /// The frequency of new paths
    /// </summary>
    public int CellPlayerMoveBeforeNewTarget = 3;       

    public bool HasBeenAwoken;
    public bool CanAttack = true;
    protected bool TargetPlayer;

    private readonly Random _rnd = new();
    private double _randomMoveTimer;
    private readonly double _randomMoveCoolDown = 1;
    private List<GameObject> _cellsInCollisionNr = new();
    private List<GameObject> _cellGameObjects = new();

    private readonly int _distanceStopFromTarget = 2; // The amount of distance the enemy has to the player

    private float _randomOffsetSeed;
    #endregion Properties

    public Enemy(GameObject gameObject) : base(gameObject)
    {
        Speed = 250;

        DamageTakenAmountTextColor = new Color[] {
            Color.OrangeRed, Color.Transparent,
        };

        Astar = new Astar();
    }

    public override void Awake()
    {
        base.Awake();

        if (WeaponGo != null)
        {
            _weaponSpriteRenderer = WeaponGo.GetComponent<SpriteRenderer>();
        }

        Collider.SetCollisionBox(15, 27, new Vector2(0, 15));
    }

    public void SetStartPosition(GameObject playerGo, Point gridPos, bool targetPlayer = true)
    {
        PlayerGo = playerGo;
        Player = playerGo.GetComponent<Player>();

        Grid = GridManager.Instance.CurrentGrid;
        GameObject.Transform.GridPosition = gridPos;

        TargetPlayer = targetPlayer;

        if (TargetPlayer)
            TargetPoint = playerGo.Transform.GridPosition;
        else
        {
            SetStartCollisionNr();
            SetCellsCollision();
            PickRandomPoint();
        }
    }

    private void SetCellsCollision()
    {
        if (Grid.CellsCollisionDict.TryGetValue(CollisionNr, out var cellList))
            _cellsInCollisionNr = cellList;
        else
            throw new Exception($"The collision Nr {CollisionNr} is not found in the saved grid");
    }

    public void SetStartEnemyRefs(List<Enemy> enemies)
    {
        _enemyListRefs = enemies.Where(enemy => enemy != this).ToList();
        Astar.UpdateEnemyListReferences(_enemyListRefs);
    }

    public override void Start()
    {
        Astar.Start(this, _enemyListRefs);

        SetRandomOffsetSeed();
        
        SetState(CharacterState.Idle);

        // Sets start position
        SetStartCollisionNr();

        Weapon.MoveWeaponAndAngle();

        if (Player.CollisionNr == CollisionNr) SetPath(); // We know that the player the targetPoint has been set
    }
    private void SetRandomOffsetSeed()
    {
        _randomOffsetSeed = (float)_rnd.NextDouble();
    }


    public override void Update()
    {
        CheckLayerDepth(); // Make sure the enemy is drawn correctly.

        //To make a new path towards the player, if they have moved.
        if (TargetPlayer) PlayerMovedInRoom();
        else RandomMoveInRoom();

        switch (State)
        {
            case CharacterState.Idle:
                ResetRotationWhenIdle();
                break;

            case CharacterState.Moving:
                UpdatePathing();
                break;

            case CharacterState.Attacking:
                ResetRotationWhenIdle();
                // Do nothing if the player has died.
                if (Player.State == CharacterState.Dead) return;

                // Update direction towards player insted of moving direction
                Direction = BaseMath.SafeNormalize(PlayerGo.Transform.Position - GameObject.Transform.Position);
                Weapon.MoveWeaponAndAngle();
                UpdateDirection();

                Attack();
                break;

            case CharacterState.Dead:
                ResetRotationWhenIdle();
                // Maybe a fade after some time?
                break;
        }
    }

    private void PlayerMovedInRoom()
    {
        if (Player.CollisionNr != CollisionNr && !HasBeenAwoken || State == CharacterState.Dead) return; // Cant move if the player isnt in the same room.

        Point playerPos = PlayerGo.Transform.GridPosition;

        // If X is more that z cells away, it should start a new target. The same with Y
        if (Player.CollisionNr == CollisionNr && !HasBeenAwoken || Math.Abs(playerPos.X - TargetPoint.X) >= CellPlayerMoveBeforeNewTarget ||
            Math.Abs(playerPos.Y - TargetPoint.Y) >= CellPlayerMoveBeforeNewTarget)
        {
            TargetPoint = playerPos;
            SetPath();
            HasBeenAwoken = true;
        }
    }

    protected virtual void RandomMoveInRoom()
    {
        if (Player.CollisionNr != CollisionNr 
            && !HasBeenAwoken 
            || State == CharacterState.Dead) return; // Cant move if the player isnt in the same room.

        if (Player.CollisionNr == CollisionNr && !HasBeenAwoken)
        {
            // Random cell
            PickRandomPoint();
            SetPath();
            _randomMoveTimer = 0;

            HasBeenAwoken = true; // Will now search each time
        }

        // Pick a random point, and set a random target
        Point curPos = GameObject.Transform.GridPosition;

        if (Math.Abs(curPos.X - TargetPoint.X) <= 1 &&
            Math.Abs(curPos.Y - TargetPoint.Y) <= 1)
        {
            _randomMoveTimer += GameWorld.DeltaTime;

            if (_randomMoveTimer < _randomMoveCoolDown) // If the timer is not ready to move yet, stay in Idle
                SetState(CharacterState.Idle);
            else // If the timer is over the cooldown, we first find a random target point and then sets the path.
            {
                // Random cell
                PickRandomPoint();
                SetPath();
                _randomMoveTimer = 0;
            }
        }
    }

    protected void PickRandomPoint()
    {
        Point randomPoint;

        int searchRadius = 1000;
        _cellGameObjects = Grid.GetCellsInRadius(GameObject.Transform.Position, searchRadius, 2);
        
        if (_cellGameObjects.Count > 0 )
            randomPoint = _cellGameObjects[_rnd.Next(0, _cellGameObjects.Count - 1)].Transform.GridPosition;
        else
            randomPoint = _cellsInCollisionNr[_rnd.Next(0, _cellsInCollisionNr.Count - 1)].Transform.GridPosition;

        GameObject currentCellGo = Grid.GetCellGameObjectFromPoint(randomPoint);
        Cell cell = currentCellGo.GetComponent<Cell>();
        if (CollisionNr != cell.CollisionNr)
        {
            randomPoint = _cellsInCollisionNr[_rnd.Next(0, _cellsInCollisionNr.Count - 1)].Transform.GridPosition;
        }

        TargetPoint = randomPoint;
    }

    private void CheckLayerDepth()
    {
        // Offset for layerdepth, so the enemies are not figting for which is shown.
        float offSet = (GameObject.Transform.Position.Y + _randomOffsetSeed) / 10_000_000f; // IMPORTANT, THIS CAN CHANGE WHAT LAYER ITS DRAWN ON
        if (GameObject.Transform.Position.Y < PlayerGo.Transform.Position.Y)
            SpriteRenderer.SetLayerDepth(LayerDepth.EnemyUnder, offSet);
        else
            SpriteRenderer.SetLayerDepth(LayerDepth.EnemyOver, offSet);
    }

    #region PathFinding
    public void SetPath()
    {
        Path = null; // We cant use the previous path
        // Create a new thread to find the path
        Path = Astar.FindPath(GameObject.Transform.GridPosition, TargetPoint);
        // Bug happened because this path got returned just as it died
        if (State == CharacterState.Dead) return;

        // If there has been found no path. Maybe here it could check astar again?
        if (Path == null || Path.Count == 0) return;

        // If a new path is being set, set the next target to the enemy's current position
        SetState(CharacterState.Moving);
        if (GameObject.Transform.Position != Path[0].Transform.Position)
            _nextTarget = GameObject.Transform.Position;
        else
            SetNextTargetPos(Path[0]);
    }

    public virtual void UpdatePathing()
    {
        if (Path == null)
            return;
        Vector2 position = GameObject.Transform.Position;

        if (Vector2.Distance(position, _nextTarget) < _thresholdToTargetCell)
        {
            bool enemyOnPath = false;
            foreach (Enemy enemy in _enemyListRefs)
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

            if (Path.Count > _distanceStopFromTarget || enemyOnPath) // If the second last in the path is the target, it should not stop inside the player
            {
                GameObject.Transform.GridPosition = Path[0].Transform.GridPosition;
                UpdateRoomNr(Path[0]);
                Path.RemoveAt(0);
                SetNextTargetPos(Path[0]);
                
            }
            else if (Path.Count == _distanceStopFromTarget) // Stops the path.
            {
                GameObject.Transform.GridPosition = Path[0].Transform.GridPosition;
                UpdateRoomNr(Path[0]);
                SetNextTargetPos(Path[0]);
                Path.RemoveAt(0);
            }
        }

        Direction = BaseMath.SafeNormalize(_nextTarget - position);

        GameObject.Transform.Translate(Direction * Speed * (float)GameWorld.DeltaTime);
        RotateCharacterOnMove(true);

        Weapon.MoveWeaponAndAngle();

        if (CanAttack && Path != null && Path.Count <= 3) // Attack before reaching the player, to put pressure on them
        {
            Direction = BaseMath.SafeNormalize(PlayerGo.Transform.Position - GameObject.Transform.Position);
            Attack();
        }
        
        if (Path.Count == 1 && Vector2.Distance(position, _nextTarget) < _thresholdToTargetCell)
        {
            if (CanAttack)
            {
                SetState(CharacterState.Attacking); // Close so would always attack
            }
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
        _nextTarget = cellGo.Transform.Position + new Vector2(0, -Cell.Dimension * Cell.Scale / 2);
    }
    #endregion PathFinding
}