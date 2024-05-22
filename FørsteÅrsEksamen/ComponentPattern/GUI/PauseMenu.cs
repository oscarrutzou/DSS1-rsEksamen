using FørsteÅrsEksamen.Factory.Gui;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.ComponentPattern.GUI
{
    public class PauseMenu
    {
        private bool showSettings;
        private enum MenuState { StartMenu, SettingsMenu }
        private MenuState currentMenuState = MenuState.StartMenu;

        private List<GameObject> startMenuObjects = new();
        private List<GameObject> settingsMenuObjects = new();

        private SpriteFont font;
        private Vector2 textPos;
        private Button musicBtn, sfxBtn;

        public void Initialize()
        {
            GlobalSounds.InMenu = true;

            font = GlobalTextures.BigFont;
            textPos = GameWorld.Instance.UiCam.Center + new Vector2(0, -200);

            //Draw background

            InitStartMenu();
            InitPauseMenu();
        }

        private void InitStartMenu()
        {
            GameObject startBtn = ButtonFactory.Create("Pause", true,
                () => { ShowHideGameObjects(startMenuObjects, false); ; });
            startMenuObjects.Add(startBtn);

            GameObject settingsBtn = ButtonFactory.Create("Settings", true,
                 () => { Settings(); });
            startMenuObjects.Add(settingsBtn);

            GameObject quitBtn = ButtonFactory.Create("Quit", true,
                 () => { GameWorld.Instance.Exit(); });
            startMenuObjects.Add(quitBtn);

            GameObject mainMenu = ButtonFactory.Create("Main Menu", true,
                () => { GameWorld.Instance.ChangeScene(ScenesNames.MainMenu); });
            startMenuObjects.Add(mainMenu);

            ShowHideGameObjects(startMenuObjects, false);

            GuiMethods.PlaceButtons(startMenuObjects, textPos + new Vector2(0, 75), 25);
        }

        private void InitPauseMenu()
        {
            GameObject musicVolGo = ButtonFactory.Create("", true, () => { ChangeMusic(); });
            musicBtn = musicVolGo.GetComponent<Button>();
            musicBtn.ChangeScale(new Vector2(14, 4));
            musicBtn.Text = $"Music Volume {GlobalSounds.MusicVolume * 100}%";
            settingsMenuObjects.Add(musicVolGo);

            GameObject sfxVolGo = ButtonFactory.Create("", true, () => { ChangeSfx(); });
            sfxBtn = sfxVolGo.GetComponent<Button>();
            sfxBtn.ChangeScale(new Vector2(14, 4));
            sfxBtn.Text = $"SFX Volume {GlobalSounds.SfxVolume * 100}%";
            settingsMenuObjects.Add(sfxVolGo);

            GameObject quitBtn = ButtonFactory.Create("Back", true, () => { Settings(); });
            settingsMenuObjects.Add(quitBtn);

            ShowHideGameObjects(settingsMenuObjects, false);

            GuiMethods.PlaceButtons(settingsMenuObjects, textPos + new Vector2(0, 75), 25);
        }

        private void ChangeMusic()
        {
            GlobalSounds.ChangeMusicVolume();
            musicBtn.Text = $"Music Volume {GlobalSounds.MusicVolume * 100}%";
        }

        private void ChangeSfx()
        {
            GlobalSounds.ChangeSfxVolume();
            sfxBtn.Text = $"SFX Volume {GlobalSounds.SfxVolume * 100}%";
        }

        public void ShowMenu()
        {
            if (currentMenuState == MenuState.StartMenu)
            {
                ShowHideGameObjects(startMenuObjects, true);
                ShowHideGameObjects(settingsMenuObjects, false);
            }
            else
            {
                ShowHideGameObjects(startMenuObjects, false);
                ShowHideGameObjects(settingsMenuObjects, true);
            }
        }

        public void HideMenu()
        {
            currentMenuState = MenuState.StartMenu;
            ShowHideGameObjects(startMenuObjects, false);
            ShowHideGameObjects(settingsMenuObjects, false);
        }

        private void Settings()
        {
            if (currentMenuState == MenuState.StartMenu)
            {
                currentMenuState = MenuState.SettingsMenu;
            }
            else
            {
                currentMenuState = MenuState.StartMenu;
            }
            ShowMenu();
        }
        private void ShowHideGameObjects(List<GameObject> gameObjects, bool isEnabled)
        {
            foreach (GameObject item in gameObjects)
            {
                item.IsEnabled = isEnabled;
            }
        }

        public void DrawOnScreen(SpriteBatch spriteBatch)
        {
            if (showSettings)
            {
                DrawMenuText(spriteBatch, "Settings", textPos);
            }
            else
            {
                DrawMenuText(spriteBatch, "Pause Menu", textPos);
            }
        }

        private void DrawMenuText(SpriteBatch spriteBatch, string text, Vector2 position)
        {
            GuiMethods.DrawTextCentered(spriteBatch, font, position, text, Color.Black);
        }


    }
}
