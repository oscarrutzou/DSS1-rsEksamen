using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons
{
    internal abstract class RangedWeapon : Weapon
    {
        private GameObject projectile; 

        protected RangedWeapon(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void MakeProjectile()
        {
            ProjectileFactory projectileFactory = new ProjectileFactory();
            projectile = projectileFactory.Create();

            GameWorld.Instance.Instantiate(projectile);
        }

        
        


    }
}