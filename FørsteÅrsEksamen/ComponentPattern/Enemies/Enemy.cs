using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.ObserverPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.ComponentPattern.Enemies
{
    //Asser, Oscar

    public enum DirectionState
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
        Dead // Where we play a dead animation, and keep the enemy on the screen
    }

    public abstract class Enemy : Component, IObserver
    {
        #region Properties
        private Vector2 direction, nextTarget, distanceToTarget;
        private GameObject playerGo;
        private Point targetPoint;

        private List<GameObject> Path { get; set; }
        internal int speed = 100; // A base speed, that will be changed on each type
        private readonly float threshold = 20f;

        public Action onGoalReached;
        internal EnemyState enemyState; // We set this in the start, so it plays the correct animation
        internal DirectionState directionState = DirectionState.Right;
        private Grid grid;

        private Astar astar;
        internal SpriteRenderer spriteRenderer;
        internal Animator animator;
        internal Collider collider;

        internal Dictionary<EnemyState, AnimNames> enemyStateAnimations = new();

        private float attackTimer;
        private readonly float attackCooldown = 2f;

        private bool inRange = false;
        private int distance;
        #endregion Properties

        public Enemy(GameObject gameObject) : base(gameObject)
        {
        }

        public void SetStartPosition(GameObject player, Point gridPos)
        {
            playerGo = player;
            targetPoint = playerGo.Transform.GridPosition;
            this.grid = GridManager.Instance.CurrentGrid;
            GameObject.Transform.GridPosition = gridPos;
        }

        public override void Awake()
        {
            base.Awake();
        }

        // Kun en gang når man finder
        // Distance check mellem playerGo.Transform.Position og enemy pos. (Vector2)
        // Indenfor rækkevide, set path, sætter state til moving og den følger pathen.

        public override void Start()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            astar = GameObject.GetComponent<Astar>();

            // Gets the animationer and adds the animations         Should be in each of the enemy start methods.
            animator = GameObject.GetComponent<Animator>();

            collider = GameObject.GetComponent<Collider>();
            collider.SetCollisionBox(10, 20);

            SetState(EnemyState.Idle); // Kommer til at slå fejl når vi flytter animationerne, da den prøver at sætte animation

            // Sets start position.
            GameObject.Transform.Position = grid.GetCellGameObjectFromPoint(GameObject.Transform.GridPosition).Transform.Position;

            SetPath(); // We know that the player the targetPoint has been set

            onGoalReached += OnGoalReached;
        }

        // Kig hvad jeg har i starten af update, husk at have de checks med og sætte targetPoint til playerGo GridPosition.
        private void ViewRange()
        {
            if (inRange == false)
            {
                if (distance > 20)
                {
                    playerGo.Transform.Position = distanceToTarget; //¨Du ska ikke sætte positionen af spilleren.
                    inRange = true;
                }
            }
            SetPath();
        }

        public override void Update(GameTime gameTime)
        {
            // Check om playerGo.Transform.GridPostion er det samme, ellers lav en ny path mod nuværende player gridposition
            if (playerGo.Transform.GridPosition != targetPoint && enemyState != EnemyState.Dead)
            {
                targetPoint = playerGo.Transform.GridPosition;
                SetPath(); //To make a new path towards the player, if they have moved.
            }

            switch (enemyState)
            {
                case EnemyState.Idle:
                    break;

                case EnemyState.Moving:
                    UpdatePathing(gameTime);
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
            // Burde ikke være attacking siden vores enemies skal også kunne gå rundt
            // Lav et tjek om vi leder efter en player eller bare går idle rundt.
            // Hvis vi skal gå idle rundt har vi brug for en liste som
            SetState(EnemyState.Attacking);

            spriteRenderer.SpriteEffects = SpriteEffects.None;
        }

        #region PathFinding

        private void SetPath()
        {
            ResetPathColor();

            Path = null; // We cant use the previous path

            Path = astar.FindPath(GameObject.Transform.GridPosition, targetPoint);

            if (Path != null && Path.Count > 0)
            {
                SetState(EnemyState.Moving);

                // If a new path is being set, set the next target to the enemy's current position
                if (GameObject.Transform.Position != Path[0].Transform.Position)
                {
                    nextTarget = GameObject.Transform.Position;
                }
                else
                {
                    SetNextTargetPos(Path[0]);
                }
            }
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
                    ResetCellColor(Path[0]);
                    Path.RemoveAt(0);
                    SetNextTargetPos(Path[0]);
                }
                else if (Path.Count == 1)
                {
                    GameObject.Transform.GridPosition = Path[0].Transform.GridPosition;
                    SetNextTargetPos(Path[0]);
                    ResetCellColor(Path[0]);
                    Path.RemoveAt(0);
                }
            }

            direction = Vector2.Normalize(nextTarget - position);

            GameObject.Transform.Translate(direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds);

            if (Path.Count == 0 && Vector2.Distance(position, nextTarget) < threshold)
            {
                onGoalReached?.Invoke();

                Path = null;
            }

            UpdateDirection();
        }

        #endregion PathFinding

        private void Attack()
        {
            attackTimer -= GameWorld.DeltaTime;

            if (attackTimer < 0)
            {
                attackTimer = attackCooldown;
            }
        }

        /// <summary>
        /// Changes the animation that plays and other variables
        /// </summary>
        /// <param name="newState"></param>
        internal void SetState(EnemyState newState)
        {
            if (enemyState == newState) return; // Dont change the state to the same and reset the animation
            enemyState = newState;

            // Something happens with the idle, it disappears for like a frame
            switch (enemyState)
            {
                case EnemyState.Idle:
                    animator.PlayAnimation(AnimNames.OrcIdle); // Hands are stuck a little over the normal sprite
                    break;

                case EnemyState.Moving:
                    animator.PlayAnimation(AnimNames.OrcRun); // Hands are stuck a little over the normal sprite
                    break;

                case EnemyState.Attacking: 
                    animator.PlayAnimation(AnimNames.OrcIdle); // Is going to animate hands too.
                    break;

                case EnemyState.Dead:
                    animator.PlayAnimation(AnimNames.OrcDeath);
                    animator.StopCurrentAnimationAtLastSprite();
                    break;
            }
        }

        private void UpdateDirection()
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
        }

        public void UpdateObserver()
        {
            // Når gridmanager ændre grid
        }
    }
}