﻿using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons
{
    internal abstract class RangedWeapon : Weapon
    {
        private GameObject projectile;        
        private bool canShoot = true;
        private float lastShot = 0;
        private float shootTimer = 1;

        protected RangedWeapon(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            lastShot += GameWorld.DeltaTime;

            if (lastShot > shootTimer)
            {
                canShoot = true;
            }
        }

        public void MakeProjectile()
        {
            //ProjectileFactory projectileFactory = new ProjectileFactory();
            projectile = ProjectileFactory.Create();
            projectile.GetComponent<Projectile>().SetValues(GameObject.Transform.Rotation);
            projectile.Transform.Position = GameObject.Transform.Position;

            GameWorld.Instance.Instantiate(projectile);
            
        }

        public void Shoot()
        {

            //projectile.GetComponent<Projectile>().SetValues(MathHelper.Pi);

            if (canShoot)
            {
                canShoot = false;
                lastShot = 0;

            }

        }



    }
}