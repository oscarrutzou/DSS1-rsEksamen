using FørsteÅrsEksamen.ComponentPattern.Enemies;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.ObserverPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.ComponentPattern.Characters
{

    // Oscar
    public class Player : Character, ISubject
    {
        private Vector2 totalInput, velocity, targetVelocity, previousPosition;

        private readonly float turnSpeed = 40f; // Adjust as needed

        internal List<IObserver> observers = new();


        public Player(GameObject gameObject) : base(gameObject)
        {
            speed = 300;
        }

        public override void Start()
        {
            spriteRenderer.SetLayerDepth(LAYERDEPTH.Player);

            animator.PlayAnimation(AnimNames.KnightIdle);
        }


        public void AddInput(Vector2 input)
        {
            if (float.IsNaN(input.X))
            {
                totalInput = Vector2.Zero;
            }
            totalInput += input;
        }

        public void Move(Vector2 input)
        {
            // Save the previous position
            previousPosition = GameObject.Transform.Position;

            // Add the additionalVelocity to the current targetVelocity
            if (input != Vector2.Zero)
            {
                targetVelocity += input.Length() > 0 ? Vector2.Normalize(input) : input;
                targetVelocity = targetVelocity.Length() > 0 ? Vector2.Normalize(targetVelocity) : targetVelocity;
            }
            else
            {
                targetVelocity = Vector2.Zero;
            }

            if (GridManager.Instance.CurrentGrid == null) return; // Player cant walk if there is no grid.

            // Velocity bliver sat til
            velocity = Vector2.Lerp(velocity, targetVelocity, turnSpeed * GameWorld.DeltaTime);

            // Separate the movement into X and Y components
            Vector2 xMovement = new Vector2(velocity.X, 0) * speed * GameWorld.DeltaTime;
            Vector2 yMovement = new Vector2(0, velocity.Y) * speed * GameWorld.DeltaTime;

            // Try moving along the X axis
            if (TryMove(xMovement))
            {
                // Update the previous position after a successful move
                previousPosition = GameObject.Transform.Position;
            }

            // Try moving along the Y axis
            if (TryMove(yMovement))
            {
                // Update the previous position after a successful move
                previousPosition = GameObject.Transform.Position;
            }

            GameWorld.Instance.WorldCam.position = GameObject.Transform.Position;

            GameObject cellGoUnderPlayer = GridManager.Instance.GetCellAtPos(GameObject.Transform.Position);
            GameObject.Transform.GridPosition = cellGoUnderPlayer.Transform.GridPosition;

            Notify();
        }

        private bool TryMove(Vector2 movement)
        {
            GameObject.Transform.Translate(movement);
            GameObject gridCell = GridManager.Instance.GetCellAtPos(GameObject.Transform.Position);
            if (gridCell == null || gridCell.GetComponent<Cell>().CellWalkableType == CellWalkableType.NotValid)
            {
                GameObject.Transform.Position = previousPosition;
                return false;
            }
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            if (totalInput != Vector2.Zero)
            {
                totalInput = Vector2.Normalize(totalInput);
            }

            Move(totalInput);
            totalInput = Vector2.Zero;
        }


        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (IObserver observer in observers)
            {
                observer.UpdateObserver();
            }
        }
    }
}