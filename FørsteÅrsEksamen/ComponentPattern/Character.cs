using FørsteÅrsEksamen.ComponentPattern.Enemies;
using FørsteÅrsEksamen.ComponentPattern.Weapons;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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

    // Oscar
    public abstract class Character : Component
    {
        #region Properties

        internal SpriteRenderer spriteRenderer;
        internal Animator animator;
        internal Collider collider;
        internal Weapon weapon;

        public GameObject WeaponGo, HandLeft, HandRight;

        internal Dictionary<CharacterState, AnimNames> characterStateAnimations = new();
        internal Vector2 idlespriteOffset = new(0, -32); // Move the animation up a bit so it looks like it walks correctly.
        internal Vector2 largeSpriteOffSet = new(0, -96); // Move the animation up more since its a 64x64 insted of 32x32 canvans, for the Run and Death.

        internal Vector2 direction;
        internal CharacterState State = CharacterState.Moving; // We use the method SetState, to we can change the animations and other variables.
        internal AnimationDirectionState directionState = AnimationDirectionState.Right;

        internal float attackTimer;
        internal float attackCooldown = 2f;

        internal int speed = 200;
        public int CurrentHealth = 100;
        public int MaxHealth = 100;

        #endregion Properties

        public Character(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            animator = GameObject.GetComponent<Animator>();
            collider = GameObject.GetComponent<Collider>();
            weapon = GameObject.GetComponent<Weapon>();
        }

        // This is not a abstract method since we only need to set it in the Player and Enemy class, and not in its subclasses
        /// <summary>
        /// A method to set the new state and change the animation drawn.
        /// </summary>
        /// <param name="newState"></param>
        internal virtual void SetState(CharacterState newState)
        { }

        /// <summary>
        /// Updates the direction of which way the sprite should draw. Remember to set the direction!
        /// </summary>
        internal virtual void UpdateDirection()
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

        // WIP

        public void DealDamage(GameObject damageGo)
        {
            Character damageGoHealth = damageGo.GetComponent<Character>();
            damageGoHealth.TakeDamage(weapon.damage);
        }

        public void TakeDamage(int damage)
        {
            int newHealth = CurrentHealth - damage;

            if (newHealth < 0) CurrentHealth = 0;
            else CurrentHealth = newHealth;

            //Delete or add to pool.
            if (CurrentHealth > 0) return;

            Enemy enemy = GameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                //    EnemyPool.Instance.ReleaseObject(GameObject);
                //}
                //else
                //{
                GameWorld.Instance.Destroy(GameObject);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 center = GameObject.Transform.Position - new Vector2(5, 5);
            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], center, null, Color.DarkRed, 0f, Vector2.Zero, 10, SpriteEffects.None, 1);
        }
    }
}