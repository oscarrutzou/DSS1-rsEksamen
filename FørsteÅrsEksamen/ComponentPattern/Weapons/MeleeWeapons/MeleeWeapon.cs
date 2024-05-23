using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.ComponentPattern.Enemies;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.ComponentPattern.Weapons.MeleeWeapons
{
    public abstract class MeleeWeapon : Weapon
    {

        protected MeleeWeapon(GameObject gameObject) : base(gameObject)
        {
        }

        protected MeleeWeapon(GameObject gameObject, bool enemyWeapon) : base(gameObject, enemyWeapon)
        {

        }

        public void DealDamage(GameObject damageGo)
        {
            Character damageGoHealth = damageGo.GetComponent<Character>();
            damageGoHealth.TakeDamage(Damage);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Attacking)
            {
                StartAttack();
                CheckCollisionAndDmg();
            }
        }

        // Attack direction
        //startAnimationAngle = GameObject.Transform.Rotation;
        //Vector2 mouseInUI = InputHandler.Instance.MouseOnUI;
        //float angleToMouse = (float)Math.Atan2(mouseInUI.Y, mouseInUI.X) + MathHelper.PiOver2;
        //startAnimationAngle = angleToMouse;
        // should lerp to the correct angle before attacking. Use a bool to see if the angle has been set

        public void CheckCollisionAndDmg()
        {
            GameObjectTypes type;
            if (EnemyWeapon)
                type = GameObjectTypes.Player;
            else
                type = GameObjectTypes.Enemy;

            foreach (GameObject otherGo in SceneData.GameObjectLists[type])
            {
                if (!otherGo.IsEnabled) continue;

                Collider otherCollider = otherGo.GetComponent<Collider>();

                if (otherCollider == null) continue;
                foreach (CollisionRectangle weaponRectangle in WeaponColliders)
                {
                    if (weaponRectangle.Rectangle.Intersects(otherCollider.CollisionBox))
                    {
                        DealDamage(otherGo);
                        break;
                    }
                }
            }
        }

        #region Weapon Colliders
        /// <summary>
        /// To make colliders for the weapon.
        /// </summary>
        /// <param name="origin">How far the origin is from the top left corner. Should have a -0.5f in X to make it centered.</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="heightFromOriginToHandle">Height from the origin on the sprite to the end of the handle</param>
        /// <param name="amountOfColliders"></param>
        protected void SetStartColliders(Vector2 origin, int width, int height, int heightFromOriginToHandle, int amountOfColliders)
        {
            spriteRenderer.OriginOffSet = origin;
            spriteRenderer.DrawPosOffSet = -origin;
            AddWeaponColliders(width, height, heightFromOriginToHandle, amountOfColliders);
        }

        /// <summary>
        /// The colliders on our weapon, used for collision between characters
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="heightFromOriginToHandle">Height from the origin on the sprite to the end of the handle</param>
        /// <param name="amountOfColliders"></param>
        private void AddWeaponColliders(int width, int height, int heightFromOriginToHandle, int amountOfColliders)
        {
            Vector2 pos = GameObject.Transform.Position;
            Vector2 scale = GameObject.Transform.Scale;

            pos += new Vector2(0, -heightFromOriginToHandle * scale.Y); // Adds the height from origin to handle

            // Adds the weapon colliders
            for (int i = 0; i < amountOfColliders; i++)
            {
                pos += new Vector2(0, -height * scale.Y);

                WeaponColliders.Add(new CollisionRectangle()
                {
                    Rectangle = MakeRec(pos, width, height, scale),
                    StartRelativePos = pos
                });
            }
        }

        private Rectangle MakeRec(Vector2 pos, int width, int height, Vector2 scale) => new Rectangle((int)pos.X, (int)pos.Y, width * (int)scale.X, (int)scale.Y * height);
        #endregion
    }
}