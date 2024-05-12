using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.ComponentPattern
{
    public enum AnimationDirectionState
    {
        Left,
        Right
    }

    public enum CharacterState
    {
        Idle,
        Moving,
        Attacking,
        Dead
    }
    public abstract class Character : Component
    {
        #region Properties
        internal SpriteRenderer spriteRenderer;
        internal Animator animator;
        internal Collider collider;

        internal Dictionary<CharacterState, AnimNames> characterStateAnimations = new();
        internal Vector2 idlespriteOffset = new(0, -32); // Move the animation up a bit so it looks like it walks correctly.
        internal Vector2 largeSpriteOffSet = new(0, -96); // Move the animation up more since its a 64x64 insted of 32x32 canvans, for the Run and Death.

        internal Vector2 direction;
        internal CharacterState charcterState; // We set this in the start, so it plays the correct animation
        internal AnimationDirectionState directionState = AnimationDirectionState.Right;

        internal float attackTimer;
        internal float attackCooldown = 2f;

        internal int speed = 100;

        #endregion

        public Character(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            animator = GameObject.GetComponent<Animator>();
            collider = GameObject.GetComponent<Collider>();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        // This is not a abstract method since we only need to set it in the Player and Enemy class, and not in its subclasses
        /// <summary>
        /// A method to set the new state and change the animation drawn.
        /// </summary>
        /// <param name="newState"></param>
        internal virtual void SetState(CharacterState newState) { }

        internal void UpdateDirection()
        {
            if (direction.X >= 0)
            {
                directionState = AnimationDirectionState.Right;
                spriteRenderer.SpriteEffects = SpriteEffects.None;
            }
            else if (direction.X < 0)
            {
                directionState = AnimationDirectionState.Left;
                spriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 center = GameObject.Transform.Position - new Vector2(5, 5);
            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], center, null, Color.DarkRed, 0f, Vector2.Zero, 10, SpriteEffects.None, 1);
        }
    }
}
