using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.ObserverPattern;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.ComponentPattern.Characters
{
    // Oscar
    public class Player : Component, ISubject
    {
        private float speed;
        private Vector2 velocity = Vector2.Zero;
        public Vector2 targetVelocity = Vector2.Zero;
        private float turnSpeed = 40f; // Adjust as needed

        public Player(GameObject gameObject) : base(gameObject)
        {
            speed = 300;
        }

        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>();
            sr.SetLayerDepth(LAYERDEPTH.Player);

            Animator animator = GameObject.GetComponent<Animator>();
            animator.PlayAnimation(AnimNames.KnightIdle);
        }

        private Vector2 totalInput = Vector2.Zero;

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

            // Velocity bliver sat til
            this.velocity = Vector2.Lerp(this.velocity, targetVelocity, turnSpeed * GameWorld.DeltaTime);

            if (float.IsNaN(velocity.X))
            {
                velocity = Vector2.Zero;
            }

            GameObject.Transform.Translate(this.velocity * speed * GameWorld.DeltaTime);

            SetGridPos();

            GameWorld.Instance.WorldCam.Move(this.velocity * speed * GameWorld.DeltaTime);

            Notify();
        }

        private void SetGridPos()
        {
            if (GridManager.Instance.CurrentGrid != null)
            {
                GameObject gridCell = GridManager.Instance.GetCellAtPos(GameObject.Transform.Position);

                if (gridCell == null) return;

                GameObject.Transform.GridPosition = gridCell.Transform.GridPosition;
            }
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

        internal List<IObserver> observers = new();

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