using FørsteÅrsEksamen.ComponentPattern.Enemies;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.ComponentPattern.Weapons.MeleeWeapons
{
    internal abstract class MeleeWeapon : Weapon
    {

       

        internal Collider collider, enemyCollider;
        internal Enemy enemy;
        internal Weapon weapon;

        private GameObject enemyGo;
        protected MeleeWeapon(GameObject gameObject) : base(gameObject)
        {
        }

     

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
        }

        public void DealDamage(GameObject damageGo)
        {

            //if (collider.CollisionBox.Intersects(enemyCollider.CollisionBox))
            //{
            Character damageGoHealth = damageGo.GetComponent<Character>();
            damageGoHealth.TakeDamage(weapon.damage);
            
            //}
        }

        public void CollidesWithGameObject()
        {
            GameObjectTypes type;
            if (enemyWeapon)
            {
                type = GameObjectTypes.Player;
            }
            else
            {
                type = GameObjectTypes.Enemy;
            }

            foreach (GameObject otherGo in SceneData.GameObjectLists[type])
            {
                if (!otherGo.IsEnabled) continue;

                Collider otherCollider = otherGo.GetComponent<Collider>();

                if (otherCollider == null) continue;
                foreach (CollisionRectangle weaponRectangle in weaponColliders)
                {
                    if (weaponRectangle.Rectangle.Intersects(otherCollider.CollisionBox))
                    {
                        DealDamage(otherGo);
                        break;
                    }
                }
                //if (thisGoCollider.CollisionBox.Intersects(otherCollider.CollisionBox))
                //{
                //    return true;
                //}
            }

            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (attacking)
            {
                CollidesWithGameObject();
            }
        }

    }
}