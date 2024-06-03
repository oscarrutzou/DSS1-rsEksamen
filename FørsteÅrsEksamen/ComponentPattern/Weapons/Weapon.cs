using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern.Enemies;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace DoctorsDungeon.ComponentPattern.Weapons
{
    /// <summary>
    /// A class used to generate a collider that moves a rotation around its startPos
    /// </summary>
    public class CollisionRectangle
    {
        public Rectangle Rectangle;
        public Vector2 StartRelativePos;
    }

    // Erik

    // Notes for what to add or change to the Weapon.
    // Only happen on attack. Also add hands. Remove it from the player and use 2 hands.
    // The hands should be given and made before making the weapon, as a part of which hands we should use.
    // Use the clenched hand for the one for the weapon and relaxed hand for the other.
    // A really nice to have to so make a trail behind the weapon when it swings:D Would be fun to make
    public abstract class Weapon : Component
    {
        public int Damage = 10;
        public Player PlayerUser { get; set; }
        public Enemy EnemyUser { get; set; }

        protected float AttackSpeed;

        protected SpriteRenderer spriteRenderer;
        // Melee weapon
        protected float LerpFromTo;
        protected float StartAnimationAngle { get; set; }
        protected float TotalLerp;
        protected float TotalElapsedTime;
        protected bool IsRotatingBack;
        protected List<CollisionRectangle> WeaponColliders = new();


        //protected bool EnemyWeapon;
        protected bool Attacking;

        protected SoundNames[] AttackSoundNames;
        protected bool PlayingSound;

        protected Vector2 StartPosOffset = new(40, 20);

        protected Weapon(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.SetLayerDepth(LayerDepth.PlayerWeapon); // Should not matter?
            spriteRenderer.IsCentered = false;

            if (EnemyUser != null)
            {
                EnemyWeaponSprite();
            }
            else
            {
                PlayerWeaponSprite();
            }
        }

        public void StartAttack()
        {
            if (Attacking) return;

            Attacking = true;
            TotalElapsedTime = 0f;

            SetAttackDirection();
        }

        protected virtual void PlayerWeaponSprite()
        { }

        protected virtual void EnemyWeaponSprite()
        { }

        protected virtual void SetAttackDirection()
        { }

        protected void PlayAttackSound()
        {
            if (PlayingSound || AttackSoundNames == null || AttackSoundNames.Length == 0) return;

            GlobalSounds.PlayRandomizedSound(AttackSoundNames, 1, 1f, true);
            PlayingSound = true;
        }

        private Vector2 lastOffSetPos, startRelativePos = new(0, 60), startRelativeOffsetPos = new Vector2(0, -20);
        public float angle; // Public for test
        protected bool LeftSide;
        public void MoveWeapon()
        {
            Vector2 userPos;
            if (EnemyUser != null)
                userPos = EnemyUser.GameObject.Transform.Position;
            else
                userPos = PlayerUser.GameObject.Transform.Position;

            if (Attacking)
            {
                // Lock the offset
                GameObject.Transform.Position = userPos + lastOffSetPos;
                return;
            }

            if (EnemyUser != null)
                angle = GetAngleToMouseEnemy(userPos);
            else
                angle = GetAngleToMousePlayer();

            // Can use lerp from the wanted move point, so its not as fast

            // Adjust the angle to be in the range of 0 to 2π
            if (angle < 0)
            {
                angle += 2 * MathHelper.Pi;
            }

            lastOffSetPos = BaseMath.Rotate(startRelativePos, angle - MathHelper.PiOver2) + startRelativeOffsetPos;
            GameObject.Transform.Position = userPos + lastOffSetPos;

            // Set the StartAnimationAngle based on the adjusted angle
            if (angle > 0.5 * MathHelper.Pi && angle < 1.5 * MathHelper.Pi)
            {
                spriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
                StartAnimationAngle = angle + MathHelper.Pi;

                LeftSide = true;
            }
            else
            {
                StartAnimationAngle = angle;
                LeftSide = false;
                spriteRenderer.SpriteEffects = SpriteEffects.None;
            }

            GameObject.Transform.Rotation = StartAnimationAngle;
        }

        private float GetAngleToMousePlayer()
        {
            Vector2 mouseInUI = InputHandler.Instance.MouseOnUI;
            return (float)Math.Atan2(mouseInUI.Y, mouseInUI.X);
        }

        private float GetAngleToMouseEnemy(Vector2 userPos)
        {
            Player target = EnemyUser.Player;

            Vector2 relativePos = target.GameObject.Transform.Position - userPos;
            return (float)Math.Atan2(relativePos.Y, relativePos.X);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //if (!InputHandler.Instance.DebugMode) return;

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