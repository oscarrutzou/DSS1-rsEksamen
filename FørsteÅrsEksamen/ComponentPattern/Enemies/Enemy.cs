using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.ObserverPattern;
using FørsteÅrsEksamen.ComponentPattern.Path;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.ComponentPattern.Enemies
{
    //Asser, Oscar
    public abstract class Enemy : Character
    {
        #region Properties
        private Grid grid;
        private Astar astar;
        private GameObject playerGo;


        private List<GameObject> path;
        private Vector2 nextTarget, distanceToTarget;
        private Point targetPoint;
        private readonly float threshold = 50f;

        public Action onGoalReached;

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
            grid = GridManager.Instance.CurrentGrid;
            GameObject.Transform.GridPosition = gridPos;
        }

        public override void Awake()
        {
            base.Awake();

            astar = GameObject.GetComponent<Astar>();

            collider.SetCollisionBox(15, 27, new Vector2(0, 15));
        }

        public override void Start()
        {
            spriteRenderer.SetLayerDepth(LAYERDEPTH.Enemies);

            SetState(CharacterState.Idle);

            // Sets start position
            GameObject.Transform.Position = grid.GetCellGameObjectFromPoint(GameObject.Transform.GridPosition).Transform.Position;

            SetPath(); // We know that the player the targetPoint has been set

            onGoalReached += OnGoalReached;
        }

        public override void Update(GameTime gameTime)
        {
            // Check om playerGo.Transform.GridPostion er det samme, ellers lav en ny path mod nuværende player gridposition
            if (playerGo.Transform.GridPosition != targetPoint && State != CharacterState.Dead)
            {
                targetPoint = playerGo.Transform.GridPosition;
                SetPath(); //To make a new path towards the player, if they have moved.
            }

            switch (State)
            {
                case CharacterState.Idle:
                    break;

                case CharacterState.Moving:
                    UpdatePathing(gameTime);
                    break;

                case CharacterState.Attacking:
                    Attack();
                    break;

                case CharacterState.Dead:
                    break;
            }
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

        private void OnGoalReached()
        {
            // Burde ikke være attacking siden vores enemies skal også kunne gå rundt
            // Lav et tjek om vi leder efter en player eller bare går idle rundt.
            // Hvis vi skal gå idle rundt har vi brug for en liste som
            SetState(CharacterState.Attacking);

            spriteRenderer.SpriteEffects = SpriteEffects.None;
        }

        #region PathFinding

        private void SetPath()
        {
            ResetPathColor();

            path = null; // We cant use the previous path

            path = astar.FindPath(GameObject.Transform.GridPosition, targetPoint);

            if (path != null && path.Count > 0)
            {
                SetState(CharacterState.Moving);

                // If a new path is being set, set the next target to the enemy's current position
                if (GameObject.Transform.Position != path[0].Transform.Position)
                {
                    nextTarget = GameObject.Transform.Position;
                }
                else
                {
                    SetNextTargetPos(path[0]);
                }
            }
        }

        private void ResetPathColor()
        {
            if (path != null)
            {
                foreach (GameObject cellGo in path)
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
            if (path == null)
                return;
            Vector2 position = GameObject.Transform.Position;

            if (Vector2.Distance(position, nextTarget) < threshold)
            {
                if (path.Count > 1)
                {
                    GameObject.Transform.GridPosition = path[0].Transform.GridPosition;
                    ResetCellColor(path[0]);
                    path.RemoveAt(0);
                    SetNextTargetPos(path[0]);
                }
                else if (path.Count == 1)
                {
                    GameObject.Transform.GridPosition = path[0].Transform.GridPosition;
                    SetNextTargetPos(path[0]);
                    ResetCellColor(path[0]);
                    path.RemoveAt(0);
                }
            }

            direction = Vector2.Normalize(nextTarget - position);

            GameObject.Transform.Translate(direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds);

            if (path.Count == 0 && Vector2.Distance(position, nextTarget) < threshold)
            {
                onGoalReached?.Invoke();

                path = null;
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

        internal override void SetState(CharacterState newState)
        {
            if (State == newState) return; // Dont change the state to the same and reset the animation
            State = newState;

            // Something happens with the idle, it disappears for like a frame
            switch (State)
            {
                case CharacterState.Idle:
                    //animator.PlayAnimation(AnimNames.OrcIdle); // Hands are stuck a little over the normal sprite
                    animator.PlayAnimation(characterStateAnimations[State]);

                    spriteRenderer.OffSet = idlespriteOffset;
                    break;

                case CharacterState.Moving:
                    //animator.PlayAnimation(AnimNames.OrcRun); // Hands are stuck a little over the normal sprite
                    animator.PlayAnimation(characterStateAnimations[State]);

                    spriteRenderer.OffSet = largeSpriteOffSet;
                    break;

                case CharacterState.Attacking: 
                    //animator.PlayAnimation(AnimNames.OrcIdle); // Is going to animate hands too.
                    animator.PlayAnimation(characterStateAnimations[CharacterState.Idle]); // Just uses the Idle since we have no attacking animation

                    spriteRenderer.OffSet = idlespriteOffset;
                    break;

                case CharacterState.Dead:
                    //animator.PlayAnimation(AnimNames.OrcDeath);
                    animator.PlayAnimation(characterStateAnimations[State]);

                    spriteRenderer.OffSet = largeSpriteOffSet;
                    animator.StopCurrentAnimationAtLastSprite();
                    break;
            }
        }

    }
}