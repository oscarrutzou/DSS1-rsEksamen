using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.ObserverPattern;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.ComponentPattern.Characters
{

    // Oscar
    public abstract class Player : Character, ISubject
    {
        internal GameObject handsGo;
        private GameObject movementColliderGo;

        private Vector2 totalMovementInput, velocity, targetVelocity, previousPosition;

        private readonly float turnSpeed = 40f; // Adjust as needed

        internal List<IObserver> observers = new();

        private Collider movementCollider;

        public Player(GameObject gameObject) : base(gameObject)
        {
            speed = 300;
        }

        public Player(GameObject gameObject, GameObject handsGo, GameObject movementColliderGo) : base(gameObject)
        {
            speed = 300;
            this.handsGo = handsGo;
            this.movementColliderGo = movementColliderGo;
        }

        public override void Awake()
        {
            base.Awake();
            movementCollider = movementColliderGo.GetComponent<Collider>();
            //movementColliderGo.Transform.Position = GameObject.Transform.Position;

            collider.SetCollisionBox(15, 24, new Vector2(0, 30));
            
        }

        public override void Start()
        {
            spriteRenderer.SetLayerDepth(LAYERDEPTH.Player);
            SetState(CharacterState.Idle);

        }


        public void AddInput(Vector2 input)
        {
            if (float.IsNaN(input.X))
            {
                totalMovementInput = Vector2.Zero;
            }
            totalMovementInput += input;
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

            bool hasMoved = false;
            // Try moving along the X axis
            if (TryMove(xMovement))
            {
                // Update the previous position after a successful move
                previousPosition = GameObject.Transform.Position;
                hasMoved = true;
            }

            // Try moving along the Y axis
            if (TryMove(yMovement))
            {
                // Update the previous position after a successful move
                previousPosition = GameObject.Transform.Position;
                hasMoved = true;
            }

            direction = velocity;
            UpdateDirection();

            if (!hasMoved) return; // Dont need to set new position, since its the same.

            SetMovement(GameObject.Transform.Position); // So we set the other gameobjects (Hands, Movement Collider...)

            // Updates the grid position
            GameObject cellGoUnderPlayer = GridManager.Instance.GetCellAtPos(GameObject.Transform.Position);
            GameObject.Transform.GridPosition = cellGoUnderPlayer.Transform.GridPosition;

            Notify();
        }

        private bool TryMove(Vector2 movement)
        {
            // Translate the GameObject, and movement collider.
            TranslateMovement(movement);

            // Get the CollisionBox
            Rectangle collisionBox = movementCollider.CollisionBox;

            // Check each corner of the CollisionBox
            Vector2[] corners = new Vector2[]
            {
                new Vector2(collisionBox.Left, collisionBox.Top),
                new Vector2(collisionBox.Right, collisionBox.Top),
                new Vector2(collisionBox.Right, collisionBox.Bottom),
                new Vector2(collisionBox.Left, collisionBox.Bottom)
            };

            foreach (Vector2 corner in corners)
            {
                GameObject gridCell = GridManager.Instance.GetCellAtPos(corner);
                if (gridCell == null || gridCell.GetComponent<Cell>().CellWalkableType == CellWalkableType.NotValid)
                {
                    // If any corner is in an invalid cell, revert the movement on both player and 
                    SetMovement(previousPosition);
                    return false;
                }
            }

            // If all corners are in valid cells, the movement is successful
            return true;
        }

        /// <summary>
        /// Adds the movement to the gameobject
        /// </summary>
        /// <param name="movement"></param>
        internal void TranslateMovement(Vector2 movement)
        {
            //GameObject.Transform.Translate(movement);
            GameObject.Transform.Position += movement;
            movementColliderGo.Transform.Position = GameObject.Transform.Position;
            handsGo.Transform.Position = GameObject.Transform.Position;
        }

        /// <summary>
        /// Sets the position of the object to the position.
        /// </summary>
        /// <param name="position"></param>
        internal void SetMovement(Vector2 position)
        {
            GameObject.Transform.Position = position;
            movementColliderGo.Transform.Position = GameObject.Transform.Position;
            handsGo.Transform.Position = GameObject.Transform.Position;
            GameWorld.Instance.WorldCam.position = GameObject.Transform.Position; //Sets the new position of the world cam

        }

        public override void Update(GameTime gameTime)
        {
            if (totalMovementInput != Vector2.Zero)
            {
                totalMovementInput = Vector2.Normalize(totalMovementInput);
                SetState(CharacterState.Moving);
            }
            else
            {
                SetState(CharacterState.Idle);

            }

            switch (State)
            {
                case CharacterState.Idle:
                    break;
                case CharacterState.Moving:
                    Move(totalMovementInput);
                    break;
                case CharacterState.Attacking:
                    break;
                case CharacterState.Dead:
                    break;

            }

            totalMovementInput = Vector2.Zero;
        }

        internal override void SetState(CharacterState newState)
        {
            if (State == newState) return; // Dont change the state to the same and reset the animation
            State = newState;

            // Something happens with the idle, it disappears for like a frame
            switch (State)
            {
                case CharacterState.Idle:
                    // Hands are stuck a little over the normal sprite
                    animator.PlayAnimation(characterStateAnimations[State]);

                    spriteRenderer.OriginOffSet = idlespriteOffset;
                    break;

                case CharacterState.Moving:
                    // Hands are stuck a little over the normal sprite
                    animator.PlayAnimation(characterStateAnimations[State]);

                    spriteRenderer.OriginOffSet = largeSpriteOffSet;
                    break;

                case CharacterState.Attacking:
                    // Is going to animate hands too.
                    animator.PlayAnimation(characterStateAnimations[CharacterState.Idle]); // Just uses the Idle since we have no attacking animation

                    spriteRenderer.OriginOffSet = idlespriteOffset;
                    break;

                case CharacterState.Dead:
                    animator.PlayAnimation(characterStateAnimations[State]);

                    spriteRenderer.OriginOffSet = largeSpriteOffSet;
                    animator.StopCurrentAnimationAtLastSprite();
                    break;
            }
        }

        #region Observer Pattern
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


        #endregion
    }
}