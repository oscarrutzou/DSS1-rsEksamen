using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.Factory.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Menus
{
    public class MainMenu : Scene
    {
        private bool showSettings;

        private List<GameObject> startMenuObjects = new();
        private List<GameObject> pauseMenuObjects = new();

        public override void Initialize()
        {
            InitStartMenu();
            InitPauseMenu();
        }

        private void InitStartMenu()
        {
            GameObject startBtn = ButtonFactory.Create("Start Game", true,
                () => { GameWorld.Instance.ChangeScene(ScenesNames.OscarTestScene); });
            startMenuObjects.Add(startBtn);

            GameObject settingsBtn = ButtonFactory.Create("Settings", true,
                 () => { Settings(); });
            startMenuObjects.Add(settingsBtn);
            
            GameObject quitBtn = ButtonFactory.Create("Quit", true,
                 () => { GameWorld.Instance.Exit(); });
            startMenuObjects.Add(quitBtn);

            GuiMethods.PlaceButtons(startMenuObjects, Vector2.Zero, 25);
        }

        private void InitPauseMenu()
        {
            GameObject settingsBtn = ButtonFactory.Create("Settings", true,
                 () => { });
            pauseMenuObjects.Add(settingsBtn);

            GameObject quitBtn = ButtonFactory.Create("Back", true,
                 () => { Settings(); });
            pauseMenuObjects.Add(quitBtn);

            ShowHideGameObjects(pauseMenuObjects, false);

            GuiMethods.PlaceButtons(pauseMenuObjects, Vector2.Zero, 25);
        }

        private void Settings()
        {
            showSettings = !showSettings;

            if (showSettings) {
                ShowHideGameObjects(startMenuObjects, false);
                ShowHideGameObjects(pauseMenuObjects, true);
            }
            else
            {
                ShowHideGameObjects(startMenuObjects, true);
                ShowHideGameObjects(pauseMenuObjects, false);
            }
        }

        private void ShowHideGameObjects(List<GameObject> gameObjects, bool isEnabled)
        {
            foreach (GameObject item in gameObjects)
            {
                item.IsEnabled = isEnabled;
            }
        }

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);

            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"{SceneData.GameObjectLists[GameObjectTypes.Gui].Count}", GameWorld.Instance.UiCam.TopLeft, Color.Black);
        }


    }
}