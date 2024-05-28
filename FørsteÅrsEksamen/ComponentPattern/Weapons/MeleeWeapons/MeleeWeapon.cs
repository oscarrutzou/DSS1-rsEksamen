using DoctorsDungeon.GameManagement.Scenes;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons
{
    public abstract class MeleeWeapon : Weapon
    {
        private List<GameObject> hitGameObjects = new();

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
            if (Attacking)
            {
                PlayAttackSound();

                TotalElapsedTime += GameWorld.DeltaTime * AttackSpeed; // To change the speed of the animation, change the attackspeed.
                AttackAnimation();

                CheckCollisionAndDmg();
            }

            UpdateCollisionBoxesPos(GameObject.Transform.Rotation);
        }

        // Attack direction
        //startAnimationAngle = GameObject.Transform.Rotation;
        //Vector2 mouseInUI = InputHandler.Instance.MouseOnUI;
        //float angleToMouse = (float)Math.Atan2(mouseInUI.Y, mouseInUI.X) + MathHelper.PiOver2;
        //startAnimationAngle = angleToMouse;
        // should lerp to the correct angle before attacking. Use a bool to see if the angle has been set

        // Need to set a bool or something to the hit objects so we stop the bug where it kills it in 10 update

        public void CheckCollisionAndDmg()
        {
            GameObjectTypes type;
            if (EnemyWeapon)
                type = GameObjectTypes.Player;
            else
                type = GameObjectTypes.Enemy;

            foreach (GameObject otherGo in SceneData.GameObjectLists[type])
            {
                if (!otherGo.IsEnabled || hitGameObjects.Contains(otherGo)) continue;

                Collider otherCollider = otherGo.GetComponent<Collider>();

                if (otherCollider == null) continue;
                foreach (CollisionRectangle weaponRectangle in WeaponColliders)
                {
                    if (weaponRectangle.Rectangle.Intersects(otherCollider.CollisionBox))
                    {
                        DealDamage(otherGo);
                        hitGameObjects.Add(otherGo);
                        break;
                    }
                }
            }

        }

        #region Weapon Colliders

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
                PlayingSound = false;
                hitGameObjects = new();
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

        #endregion Weapon Colliders
    }
}