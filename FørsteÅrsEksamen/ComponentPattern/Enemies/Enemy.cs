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
        public Point targetCharacter;
        public List<GameObject> Path { get; set; }
        public int speed;
        private readonly float threshold = 5f;

        public Action onGoalReached;
        public EnemyState enemyState = EnemyState.Inactive;
        public DirectionState directionState = DirectionState.Right;
        private Grid grid;
        private Astar astar;

        private SpriteRenderer spriteRenderer;
        private float attackTimer;
        private readonly float attackCooldown = 2f;
        public Enemy(GameObject gameObject) : base(gameObject)
        {

        }

        public void SetStartPosition(Grid grid, Point gridPos)
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

                Path = astar.FindPath(GameObject.Transform.GridPosition, targetCharacter);
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
