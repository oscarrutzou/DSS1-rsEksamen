using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Weapons;
using FørsteÅrsEksamen.Factory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FørsteÅrsEksamen.GameManagement.Scenes
{
    public class ErikTestScene : Scene
    {
        private GameObject weapon;
        public override void Initialize()
        {
            MakeWeapon();
            AttackCommand();
        }

        private void MakeWeapon()
        {
            WeaponFactory weaponFactory = new();
            GameObject weapon = weaponFactory.Create();
            
            GameWorld.Instance.Instantiate(weapon);
        }

       private void AttackCommand()
        {
        
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