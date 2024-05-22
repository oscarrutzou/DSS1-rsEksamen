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
        private GameObject bow;
        private GameObject arrow;

        private bool canShoot = true;
        private float lastShot = 0;
        private float shootTimer = 1;

        public override void Initialize()
        {
            MakeWeapon();
            GameWorld.Instance.WorldCam.position = Vector2.Zero;
            
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
            
            bow = WeaponFactory.Create(WeaponTypes.Bow);
            
            
            GameWorld.Instance.Instantiate(weapon);
            GameWorld.Instance.Instantiate(bow);
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

        public void Shoot()
        {
           var rangedWeapon = bow.GetComponent<RangedWeapon>();  

            //projectile.GetComponent<Projectile>().SetValues(MathHelper.Pi);

            if (rangedWeapon != null)
            {

                rangedWeapon.Shoot();
                

            }

        }


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