using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.ComponentPattern.Weapons
{
    /// <summary>
    /// A class used to generate a collider that moves a rotation around its startPos
    /// </summary>
    public class CollisionRectangle
    {
        public Rectangle Rectangle;
        public Vector2 StartRelativePos;
    }

    // Only happen on attack. Also add hands. Remove it from the player and use 2 hands.
    // The hands should be given and made before making the weapon, as a part of which hands we should use.
    // Use the clenched hand for the one for the weapon and relaxed hand for the other.
    // Make it easy to set how much it should lerp to and from.
    // Make it wait for some time (need to be able to be set) before roating back.

    // Maybe as a nice to have, look into how to make one of the less smooth path. So it e.g is fast in the beginning and slow at the end.
    // A really nice to have to so make a trail behind the weapon when it swings:D Would be fun to make
    public abstract class Weapon : Component
    {
        public int Damage = 10;

        protected float AttackSpeed;

        protected SpriteRenderer spriteRenderer;
        protected float LerpFromTo = MathHelper.Pi;
        protected float TotalLerp;

        protected bool EnemyWeapon;
        protected bool Attacking;
        protected float StartAnimationAngle;

        protected float TotalElapsedTime;
        protected bool IsRotatingBack;
        protected List<CollisionRectangle> WeaponColliders = new();

        protected SoundNames[] AttackSoundNames;

        protected Weapon(GameObject gameObject) : base(gameObject)
        {
            AttackSpeed = 1.7f;
        }

        protected Weapon(GameObject gameObject, bool enemyWeapon) : base(gameObject)
        {
            AttackSpeed = 1.7f;
            this.EnemyWeapon = enemyWeapon;
        }

        public override void Awake()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.SetLayerDepth(LAYERDEPTH.Player);
            spriteRenderer.IsCentered = false;
        }       

        public virtual void StartAttack() { }

        private bool playingSound = false;

        public override void Update(GameTime gameTime)
        {
            if (Attacking)
            {
                PlayAttackSound();

                //Move a lot to attack method.
                TotalElapsedTime += GameWorld.DeltaTime * AttackSpeed; // To change the speed of the animation, change the attackspeed.
                AttackAnimation();

            }

            UpdateCollisionBoxesPos(GameObject.Transform.Rotation);
        }

        private void PlayAttackSound()
        {
            if (playingSound || AttackSoundNames == null || AttackSoundNames.Length == 0) return;

            GlobalSounds.PlayRandomizedSound(AttackSoundNames, 1, 1f, true);
            playingSound = true;
        }

        public void MoveWeapon(Vector2 movePos) => GameObject.Transform.Position = movePos;

        private void AttackAnimation()
        {
            if (TotalElapsedTime >= 1f)
            {
                TotalElapsedTime = 0f; // Reset totalElapsedTime
                IsRotatingBack = true;
            }

            // Play with some other methods, for different weapons, to make them feel slow or fast https://easings.net/
            float easedTime; // maybe switch between them.

            if (!IsRotatingBack)
            {
                // Down attack
                easedTime = BaseMath.EaseInOutBack(TotalElapsedTime);
                GameObject.Transform.Rotation = MathHelper.Lerp(StartAnimationAngle, TotalLerp, easedTime);
            }
            else
            {
                //Up attack
                easedTime = BaseMath.EaseInOutBack(TotalElapsedTime);
                //easedTime = EaseInOutBack(totalElapsedTime); // Feels heavy
                GameObject.Transform.Rotation = MathHelper.Lerp(TotalLerp, StartAnimationAngle, easedTime);
            }
            if (Math.Abs(GameObject.Transform.Rotation - StartAnimationAngle) < 0.01f && IsRotatingBack)
            {
                IsRotatingBack = false;
                Attacking = false;
                playingSound = false;
            }
        }

        private void UpdateCollisionBoxesPos(float rotation)
        {
            foreach (CollisionRectangle collisionRectangle in WeaponColliders)
            {
                // Calculate the position relative to the center of the weapon
                Vector2 relativePos = collisionRectangle.StartRelativePos;

                // Rotate the relative position
                Vector2 newPos = BaseMath.Rotate(relativePos, rotation);

                // Set the collision rectangle position based on the rotated relative position
                collisionRectangle.Rectangle.X = (int)(GameObject.Transform.Position.X + newPos.X) - collisionRectangle.Rectangle.Width / 2;
                collisionRectangle.Rectangle.Y = (int)(GameObject.Transform.Position.Y + newPos.Y) - collisionRectangle.Rectangle.Height / 2;
            }
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!InputHandler.Instance.DebugMode) return;

            foreach (CollisionRectangle collisionRectangle in WeaponColliders)
            {
                Collider.DrawRectangleNoSprite(collisionRectangle.Rectangle, Color.Black, spriteBatch);
            }

            Vector2 center = GameObject.Transform.Position - new Vector2(5, 5);
            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], center, null, Color.Red, GameObject.Transform.Rotation, Vector2.Zero, 10, SpriteEffects.None, 1);
            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], center, null, Color.Pink, GameObject.Transform.Rotation, Vector2.Zero, 10, SpriteEffects.None, 1);
        }
    }
}