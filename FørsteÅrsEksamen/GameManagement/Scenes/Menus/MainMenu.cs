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

        private SpriteFont font;
        private Vector2 textPos;

        public override void Initialize()
        {
            GlobalSounds.InMenu = true;

            font = GlobalTextures.DefaultFont;
            textPos = GameWorld.Instance.UiCam.Center + new Vector2(0, -200);
            
            //Draw background

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

            GuiMethods.PlaceButtons(startMenuObjects, textPos + new Vector2(0, 75), 25);
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

            GuiMethods.PlaceButtons(pauseMenuObjects, textPos + new Vector2(0, 75), 25);
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

            if (showSettings)
            {
                DrawMenuText(spriteBatch, "Settings", textPos);
            }
            else
            {
                DrawMenuText(spriteBatch, "Doctor's Dunguon", textPos);
            }

            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"{SceneData.GameObjectLists[GameObjectTypes.Gui].Count}", GameWorld.Instance.UiCam.TopLeft, Color.Black);
        }

        private void DrawMenuText(SpriteBatch spriteBatch, string text, Vector2 position)
        {
            GuiMethods.DrawTextCentered(spriteBatch, font, position, text, Color.Black);
        }


    }
}