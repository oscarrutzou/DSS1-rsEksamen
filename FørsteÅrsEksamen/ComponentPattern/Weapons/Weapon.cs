using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        protected float StartAnimationAngle;

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

        private Vector2 lastOffSet;

        public void MoveWeapon()
        {
            Vector2 userPos = WeaponUser.GameObject.Transform.Position;

            if (Attacking)
            {
                // Lock the offset
                GameObject.Transform.Position = userPos + lastOffSet;
                return;
            }

            if (WeaponUser.Direction.X >= 0)
            {
                // Right
                lastOffSet = new Vector2(StartPosOffset.X, -StartPosOffset.Y);
                GameObject.Transform.Position = userPos + lastOffSet;
                spriteRenderer.SpriteEffects = SpriteEffects.None;
            }
            else if (WeaponUser.Direction.X < 0)
            {
                lastOffSet = -StartPosOffset;
                GameObject.Transform.Position = userPos + lastOffSet;
                spriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
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