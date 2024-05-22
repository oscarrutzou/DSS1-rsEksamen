using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Rooms
{
    public class Room1Scene : Scene
    {
        private PauseMenu pauseMenu;
        private bool isMenuVisible;

        public Room1Scene()
        {
            PlayerSpawnPos = new Point(5, 5);
        }
        public override void Initialize()
        {
            pauseMenu = new PauseMenu();
            pauseMenu.Initialize();
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(TogglePauseMenu));
        }

        private void TogglePauseMenu()
        {
            isMenuVisible = !isMenuVisible;
            if (isMenuVisible)
            {
                pauseMenu.ShowMenu();
            }
            else
            {
                pauseMenu.HideMenu();
            }
        }

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);
            if (isMenuVisible)
            {
                pauseMenu.DrawOnScreen(spriteBatch);
            }
        }
    }
}