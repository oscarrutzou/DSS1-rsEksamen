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
        public Character WeaponUser;

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
        protected bool PlayingSound;

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

        protected virtual void PlayerStartAttack() {}
        protected virtual void EnemyStartAttack() { }

        protected void PlayAttackSound()
        {
            if (PlayingSound || AttackSoundNames == null || AttackSoundNames.Length == 0) return;

            GlobalSounds.PlayRandomizedSound(AttackSoundNames, 1, 1f, true);
            PlayingSound = true;
        }

        public void MoveWeapon(Vector2 movePos) => GameObject.Transform.Position = movePos;

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