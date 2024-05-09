using FørsteÅrsEksamen.ComponentPattern.Grid;
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
        public Point targetCharacter;
        public List<GameObject> path { get; set; }
        public int speed;
        private float threshold = 5f;

        public Action onGoalReached;
        public EnemyState enemyState = EnemyState.Inactive;
        public DirectionState directionState = DirectionState.Right;
        private Grid.Grid grid;
        private Astar astar;

        private SpriteRenderer spriteRenderer;
        private float attackTimer;
        private readonly float attackCooldown = 2f;
        public Enemy(GameObject gameObject) : base(gameObject)
        {

        }

        public void SetStartPosition(Grid.Grid grid, Point gridPos)
        {
            this.grid = grid;
            GameObject.Transform.GridPosition = gridPos;
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
            if (path.Count > 0)
            {
                SetNextTargetPos(path[0]);
            }
        }

        private void MakePath()
        {
            for (int i = 0; i < 3; i++)
            {
                if (path != null && path.Count > 0) break;

                path = astar.FindPath(GameObject.Transform.GridPosition, targetCharacter);
            }
            if (path == null) throw new Exception("No path available");
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
                    path.RemoveAt(0);
                    SetNextTargetPos(path[0]);
                }
                else if (path.Count == 1)
                {
                    GameObject.Transform.GridPosition = path[0].Transform.GridPosition;
                    SetNextTargetPos(path[0]);
                    path.RemoveAt(0);
                }
            }

            direction = Vector2.Normalize(nextTarget - position);

            GameObject.Transform.Translate(direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds);

            if (path.Count == 0 && Vector2.Distance(position, nextTarget) < threshold)
            {
                if (onGoalReached != null)
                {
                    onGoalReached.Invoke();
                    onGoalReached = null;
                }
                path = null;
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
