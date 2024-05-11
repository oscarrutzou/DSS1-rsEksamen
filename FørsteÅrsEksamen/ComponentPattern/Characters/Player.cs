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
    public class Player : Component, ISubject
    {
        internal SpriteRenderer spriteRenderer;
        internal Animator animator;

        private float speed;
        private Vector2 totalInput;
        private Vector2 velocity;
        public Vector2 targetVelocity;
        private float turnSpeed = 40f; // Adjust as needed

        internal Dictionary<EnemyState, AnimNames> enemyStateAnimations = new();
        private Vector2 idlespriteOffset = new(0, -32); // Move the animation up a bit so it looks like it walks correctly.
        private Vector2 largeSpriteOffSet = new(0, -96); // Move the animation up more since its a 64x64 insted of 32x32 canvans, for the Run and Death.


        public Player(GameObject gameObject) : base(gameObject)
        {
            speed = 300;
        }

        public override void Start()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.SetLayerDepth(LAYERDEPTH.Player);

            animator = GameObject.GetComponent<Animator>();
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

        //private Vector2 previousPosition;
        //public void Move(Vector2 input)
        //{
        //    // Save the previous position
        //    previousPosition = GameObject.Transform.Position;

        //    // Add the additionalVelocity to the current targetVelocity
        //    if (input != Vector2.Zero)
        //    {
        //        targetVelocity += input.Length() > 0 ? Vector2.Normalize(input) : input;
        //        targetVelocity = targetVelocity.Length() > 0 ? Vector2.Normalize(targetVelocity) : targetVelocity;
        //    }
        //    else
        //    {
        //        targetVelocity = Vector2.Zero;
        //    }

        //    if (GridManager.Instance.CurrentGrid == null) return; // Player cant walk if there is no grid.

        //    // Velocity bliver sat til
        //    velocity = Vector2.Lerp(velocity, targetVelocity, turnSpeed * GameWorld.DeltaTime);

        //    // Separate the movement into X and Y components
        //    Vector2 xMovement = new Vector2(velocity.X, 0) * speed * GameWorld.DeltaTime;
        //    Vector2 yMovement = new Vector2(0, velocity.Y) * speed * GameWorld.DeltaTime;

        //    // Try moving along the X axis
        //    if (TryMove(xMovement))
        //    {
        //        // Update the previous position after a successful move
        //        previousPosition = GameObject.Transform.Position;
        //    }

        //    // Try moving along the Y axis
        //    if (TryMove(yMovement))
        //    {
        //        // Update the previous position after a successful move
        //        previousPosition = GameObject.Transform.Position;
        //    }


        //    // Only move the camera if the player has moved
        //    if (GameObject.Transform.Position != previousPosition)
        //    {
        //        return;
        //    }


        //    GameWorld.Instance.WorldCam.Move(velocity * speed * GameWorld.DeltaTime);

        //    GameObject cellGoUnderPlayer = GridManager.Instance.GetCellAtPos(GameObject.Transform.Position);
        //    GameObject.Transform.GridPosition = cellGoUnderPlayer.Transform.GridPosition;

        //    Notify();
        //}

        //private bool TryMove(Vector2 movement)
        //{
        //    GameObject.Transform.Translate(movement);
        //    GameObject gridCell = GridManager.Instance.GetCellAtPos(GameObject.Transform.Position);
        //    if (gridCell == null || gridCell.GetComponent<Cell>().CellWalkableType == CellWalkableType.NotValid)
        //    {
        //        GameObject.Transform.Position = previousPosition;
        //        return false;
        //    }
        //    return true;
        //}

        public void Move(Vector2 input)
        {
            // Save the previous position
            Vector2 previousPosition = GameObject.Transform.Position;

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


            velocity = Vector2.Lerp(velocity, targetVelocity, turnSpeed * GameWorld.DeltaTime);

            GameObject.Transform.Translate(velocity * speed * GameWorld.DeltaTime);

            // Check if the new position can be move to, else dont move there
            GameObject gridCell = GridManager.Instance.GetCellAtPos(GameObject.Transform.Position);

            if (gridCell == null || gridCell.GetComponent<Cell>().CellWalkableType == CellWalkableType.NotValid)
            {
                GameObject.Transform.Position = previousPosition;
                return;
            }

            GameObject.Transform.GridPosition = gridCell.Transform.GridPosition;


            GameWorld.Instance.WorldCam.Move(velocity * speed * GameWorld.DeltaTime);

            Notify();
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 center = GameObject.Transform.Position - new Vector2(5, 5);
            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], center, null, Color.DarkRed, 0f, Vector2.Zero, 10, SpriteEffects.None, 1);
        }
    }
}