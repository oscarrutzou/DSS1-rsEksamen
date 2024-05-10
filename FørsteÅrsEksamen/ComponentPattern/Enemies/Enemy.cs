using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace FørsteÅrsEksamen.ComponentPattern.Enemies
{
    //Asser

    public enum DirectionState
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum EnemyState
    {
        Inactive,
        Moving,
        Attacking,
        Dead
    }

    public abstract class Enemy : Component, IEnemyAction
    {

        private Vector2 direction, nextTarget;
        private GameObject playerGo;
        public Point targetPoint;
        public List<GameObject> Path { get; set; }
        public int speed;
        private readonly float threshold = 5f;

        public Action onGoalReached;
        public EnemyState enemyState = EnemyState.Inactive;
        public DirectionState directionState = DirectionState.Right;
        private Grid grid;
        private Astar astar;

        internal SpriteRenderer spriteRenderer;
        internal Animator animator;
        private float attackTimer;
        private readonly float attackCooldown = 2f;

        public Enemy(GameObject gameObject) : base(gameObject)
        {

        }

        public void SetStartPosition(GameObject player, Grid grid, Point gridPos)
        {
            playerGo = player;
            targetPoint = playerGo.Transform.GridPosition;
            this.grid = grid;
            GameObject.Transform.GridPosition = gridPos;
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            astar = GameObject.GetComponent<Astar>();

            Animator animator = GameObject.GetComponent<Animator>();
            animator.AddAnimation(AnimNames.OrcIdle);
            animator.PlayAnimation(AnimNames.OrcIdle);

            GameObject.Transform.Position = grid.GetCellGameObjectFromPoint(GameObject.Transform.GridPosition).Transform.Position;

            onGoalReached += OnGoalReached;
        }

        public override void Update(GameTime gameTime)
        {
            // Kun en gang når man finder
            // Distance check mellem playerGo.Transform.Position og enemy pos. (Vector2) 
            // Indenfor rækkevide, set path, sætter state til moving og den følger pathen.


            // Check om playerGo.Transform.GridPostion er det samme, ellers lav en ny path mod nuværende player gridposition
            if (playerGo.Transform.GridPosition != targetPoint)
            {
                targetPoint = playerGo.Transform.GridPosition;
                //SetPath();
            }

            switch (enemyState)
            {
                case EnemyState.Inactive:
                    break;
                case EnemyState.Moving:
                    //UpdatePathing(gameTime);
                    break;
                case EnemyState.Attacking:
                    Attack();
                    break;
                case EnemyState.Dead:
                    break;
                default:
                    break;
            }
        }

        private void OnGoalReached()
        {
            enemyState = EnemyState.Attacking;
            spriteRenderer.SpriteEffects = SpriteEffects.None;
        }

        #region PathFinding
        private void SetPath()
        {
            MakePath();
            if (Path.Count > 0)
            {
                SetNextTargetPos(Path[0]);
            }
        }

        private void MakePath()
        {
            for (int i = 0; i < 3; i++)
            {
                if (Path != null && Path.Count > 0) break;

                Path = astar.FindPath(GameObject.Transform.GridPosition, targetPoint);
            }
            if (Path == null) throw new Exception("No path available");
        }

        private void UpdatePathing(GameTime gameTime)
        {
            if (Path == null)
                return;
            Vector2 position = GameObject.Transform.Position;

            if (Vector2.Distance(position, nextTarget) < threshold)
            {
                if (Path.Count > 1)
                {
                    GameObject.Transform.GridPosition = Path[0].Transform.GridPosition;
                    Path.RemoveAt(0);
                    SetNextTargetPos(Path[0]);
                }
                else if (Path.Count == 1)
                {
                    GameObject.Transform.GridPosition = Path[0].Transform.GridPosition;
                    SetNextTargetPos(Path[0]);
                    Path.RemoveAt(0);
                }
            }

            direction = Vector2.Normalize(nextTarget - position);

            GameObject.Transform.Translate(direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds);

            if (Path.Count == 0 && Vector2.Distance(position, nextTarget) < threshold)
            {
                if (onGoalReached != null)
                {
                    onGoalReached.Invoke();
                    onGoalReached = null;
                }
                Path = null;
            }
            UpdateDirection();
        }

        private void SetNextTargetPos(GameObject cellGo)
        {
            nextTarget = cellGo.Transform.Position + new Vector2(0, -Cell.dimension / 2);
        }
        #endregion
        
        private void Attack()
        {
            attackTimer -= GameWorld.DeltaTime;

            if (attackTimer < 0)
            {
                attackTimer = attackCooldown;

            }
        }

        public void UpdateDirection()
        {
            if (direction.X >= 0)
            {
                directionState = DirectionState.Right;
                spriteRenderer.SpriteEffects = SpriteEffects.None;
            }
            else if (direction.X < 0)
            {
                directionState = DirectionState.Left;
                spriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
            }
            else if (direction.Y > 0)
            {
                directionState = DirectionState.Down;
            }
            else if (direction.Y < 0)
            {
                directionState = DirectionState.Up;
            }
        }
    }
}
