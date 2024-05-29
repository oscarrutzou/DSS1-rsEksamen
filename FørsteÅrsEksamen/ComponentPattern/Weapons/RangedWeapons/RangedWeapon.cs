using DoctorsDungeon.Factory;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.Weapons.RangedWeapons
{
    // Erik
    public abstract class RangedWeapon : Weapon
    {
        private GameObject projectile;
        private bool canShoot = true;
        private float lastShot = 0;
        private float shootTimer = 0.5f;

        protected RangedWeapon(GameObject gameObject) : base(gameObject)
        {
        }

        protected RangedWeapon(GameObject gameObject, bool enemyWeapon) : base(gameObject, enemyWeapon)
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
                MakeProjectile();
            }
        }
    }
}