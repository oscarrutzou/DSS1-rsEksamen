using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.GameManagement.Scenes.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Rooms
{
    public class Room1Scene : Scene
    {
        private PauseMenu pauseMenu;

        public Room1Scene()
        {
            PlayerSpawnPos = new Point(5, 5);
        }

        public override void Initialize()
        {
            pauseMenu = new PauseMenu();
            pauseMenu.Initialize();
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(pauseMenu.TogglePauseMenu));
        }

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);

            pauseMenu.DrawOnScreen(spriteBatch);
        }
    }
}