using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Weapons;
using FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons;
using FørsteÅrsEksamen.Factory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FørsteÅrsEksamen.GameManagement.Scenes
{
    public class ErikTestScene : Scene
    {
        private GameObject weapon;
        private GameObject projectile;
        public override void Initialize()
        {
            //MakeWeapon();
            MakeProjectile();
            AttackCommand();
        }

        private void MakeWeapon()
        {
            weapon = WeaponFactory.Create(WeaponTypes.Sword);
            
            GameWorld.Instance.Instantiate(weapon);
        }

        private void MakeProjectile()
        {
            ProjectileFactory projectileFactory = new();
            projectile = projectileFactory.Create();

            GameWorld.Instance.Instantiate(projectile);
        }

        private void Attack()
        {
            weapon.GetComponent<MagicStaff>().Attack();
        }
       private void AttackCommand()
        {
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space, 
                new CustomCmd(Attack));


        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void DrawInWorld(SpriteBatch spriteBatch)
        {
            base.DrawInWorld(spriteBatch);
        }

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);
        }
    }
}