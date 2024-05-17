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
        
        private bool canShoot = true;
        private float lastShot = 0;
        private float shootTimer = 1;
        public override void Initialize()
        {
            //MakeWeapon();
            GameWorld.Instance.WorldCam.position = Vector2.Zero;
            MakeProjectile();
            AttackCommand();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //lastShot += GameWorld.DeltaTime;

            //if (lastShot > shootTimer)
            //{
            //    canShoot = true;
            //}
        }

        private void MakeWeapon()
        {
            weapon = WeaponFactory.Create(WeaponTypes.Sword);
            
            GameWorld.Instance.Instantiate(weapon);
        }

        private void MakeProjectile()
        {
            ProjectileFactory projectileFactory = new ProjectileFactory();
            projectile = projectileFactory.Create();

            GameWorld.Instance.Instantiate(projectile);
        }

        private void Attack()
        {
            weapon.GetComponent<Weapon>().Attack();
            //projectile.GetComponent<MagicStaff>().Attack();
        }
       private void AttackCommand()
        {
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space, 
                new CustomCmd(Attack));

            InputHandler.Instance.AddKeyButtonDownCommand(Keys.B,
                new CustomCmd(Shoot));


        }

        private void Shoot() { 
        
            projectile.GetComponent<Projectile>().SetValues(MathHelper.Pi);

        }

        //private void Shoot()
        //{
        //    if (canShoot)
        //    {
        //        canShoot = false;
        //        lastShot = 0;

        //    }

        //}



        public override void DrawInWorld(SpriteBatch spriteBatch)
        {
            base.DrawInWorld(spriteBatch);

            //spriteBatch.Draw(GlobalTextures.Textures[TextureNames.WoodSword], Vector2.Zero, Color.White);
        }

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);
        }
    }
}