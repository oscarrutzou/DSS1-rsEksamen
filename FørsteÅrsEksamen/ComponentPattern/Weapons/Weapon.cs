using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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
        public Character WeaponUser { get; set; }

        protected float AttackSpeed;

        protected SpriteRenderer spriteRenderer;
        protected float LerpFromTo;
        protected float TotalLerp;

        protected bool EnemyWeapon;
        protected bool Attacking;
        protected float StartAnimationAngle { get; set; }

        protected float TotalElapsedTime;
        protected bool IsRotatingBack;
        protected List<CollisionRectangle> WeaponColliders = new();

        protected SoundNames[] AttackSoundNames;
        protected bool PlayingSound;

        protected Vector2 StartPosOffset = new(40, 20);

        protected Weapon(GameObject gameObject) : base(gameObject)
        {
        }

        protected Weapon(GameObject gameObject, bool enemyWeapon) : base(gameObject)
        {
            this.EnemyWeapon = enemyWeapon;
        }

        public override void Awake()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.SetLayerDepth(LayerDepth.PlayerWeapon);
            spriteRenderer.IsCentered = false;

            if (EnemyWeapon)
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

            if (EnemyWeapon)
            {
                EnemyStartAttack();
            }
            else
            {
                PlayerStartAttack();
            }
        }

        protected virtual void PlayerWeaponSprite()
        { }

        protected virtual void EnemyWeaponSprite()
        { }

        protected virtual void PlayerStartAttack()
        { }

        protected virtual void EnemyStartAttack()
        { }

        protected void PlayAttackSound()
        {
            if (PlayingSound || AttackSoundNames == null || AttackSoundNames.Length == 0) return;

            GlobalSounds.PlayRandomizedSound(AttackSoundNames, 1, 1f, true);
            PlayingSound = true;
        }

        private Vector2 lastOffSetPos, startRelativePos = new(0, 70);
        public float angleToMouse;
        public void MoveWeapon()
        {
            Vector2 userPos = WeaponUser.GameObject.Transform.Position;

            if (Attacking)
            {
                // Lock the offset
                GameObject.Transform.Position = userPos + lastOffSetPos;
                return;
            }

            if (EnemyWeapon) return; //

            // Only for player 

            // Weapon
            // need to make a start position that are like 80px from this postion and in a radius around the player
            // use rotate method to get the ned point. After we have done that, we need to rotate it a bit
            Vector2 mouseInUI = InputHandler.Instance.MouseOnUI;
            angleToMouse = (float)Math.Atan2(mouseInUI.Y, mouseInUI.X);

            // Adjust the angle to be in the range of 0 to 2π
            if (angleToMouse < 0)
            {
                angleToMouse += 2 * MathHelper.Pi;
            }

            lastOffSetPos = BaseMath.Rotate(startRelativePos, angleToMouse - MathHelper.PiOver2) + new Vector2(0, -20);
            GameObject.Transform.Position = userPos + lastOffSetPos;

            // Set the StartAnimationAngle based on the adjusted angle
            if (angleToMouse > 0.5 * MathHelper.Pi && angleToMouse < 1.5 * MathHelper.Pi)
            {
                StartAnimationAngle = MathHelper.PiOver4;
            }
            else
            {
                StartAnimationAngle = -MathHelper.PiOver4;
            }

            GameObject.Transform.Rotation = StartAnimationAngle;
            // Mellem 0.5-1.5pi

            // Can use lerp from the wanted move point, so its not as fast
        }

        //if (WeaponUser.Direction.X >= 0)
        //{
        //    // Right
        //    lastOffSet = new Vector2(StartPosOffset.X, -StartPosOffset.Y);
        //    GameObject.Transform.Position = userPos + lastOffSet;
        //    spriteRenderer.SpriteEffects = SpriteEffects.None;
        //}
        //else if (WeaponUser.Direction.X < 0)
        //{
        //    lastOffSet = -StartPosOffset;
        //    GameObject.Transform.Position = userPos + lastOffSet;
        //    spriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
        //}


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