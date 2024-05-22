using FørsteÅrsEksamen.Factory.Gui;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.GUI;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Menus
{
    public class PauseMenu : MenuScene
    {
        private bool isMenuVisible;
        private enum MenuState { StartMenu, SettingsMenu }
        private MenuState currentMenuState = MenuState.StartMenu;

        protected override void InitStartMenu()
        {
            GameObject startBtn = ButtonFactory.Create("Pause", true, TogglePauseMenu);
            StartMenuObjects.Add(startBtn);

            GameObject settingsBtn = ButtonFactory.Create("Settings", true, Settings);
            StartMenuObjects.Add(settingsBtn);

            GameObject quitBtn = ButtonFactory.Create("Quit", true, GameWorld.Instance.Exit);
            StartMenuObjects.Add(quitBtn);

            GameObject mainMenu = ButtonFactory.Create("Main Menu", true,
                () => { GameWorld.Instance.ChangeScene(ScenesNames.MainMenu); });
            StartMenuObjects.Add(mainMenu);

            ShowHideGameObjects(StartMenuObjects, false);

            GuiMethods.PlaceButtons(StartMenuObjects, TextPos + new Vector2(0, 75), 25);
        }

        protected override void InitSettingsMenu()
        {
            GameObject musicVolGo = ButtonFactory.Create("", true, ChangeMusic);
            MusicBtn = musicVolGo.GetComponent<Button>();
            MusicBtn.ChangeScale(new Vector2(14, 4));
            MusicBtn.Text = $"Music Volume {GlobalSounds.MusicVolume * 100}%";
            PauseMenuObjects.Add(musicVolGo);

            GameObject sfxVolGo = ButtonFactory.Create("", true, ChangeSfx);
            SfxBtn = sfxVolGo.GetComponent<Button>();
            SfxBtn.ChangeScale(new Vector2(14, 4));
            SfxBtn.Text = $"SFX Volume {GlobalSounds.SfxVolume * 100}%";
            PauseMenuObjects.Add(sfxVolGo);

            GameObject quitBtn = ButtonFactory.Create("Back", true, Settings);
            PauseMenuObjects.Add(quitBtn);

            ShowHideGameObjects(PauseMenuObjects, false);

            GuiMethods.PlaceButtons(PauseMenuObjects, TextPos + new Vector2(0, 75), 25);
        }

        public void TogglePauseMenu()
        {
            isMenuVisible = !isMenuVisible;
            if (isMenuVisible)
            {
                ShowMenu();
            }
            else
            {
                HideMenu();
            }
        }

        private void ShowMenu()
        {
            if (currentMenuState == MenuState.StartMenu)
            {
                ShowHideGameObjects(StartMenuObjects, true);
                ShowHideGameObjects(PauseMenuObjects, false);
            }
            else
            {
                ShowHideGameObjects(StartMenuObjects, false);
                ShowHideGameObjects(PauseMenuObjects, true);
            }
        }

        private void HideMenu()
        {
            currentMenuState = MenuState.StartMenu;
            ShowHideGameObjects(StartMenuObjects, false);
            ShowHideGameObjects(PauseMenuObjects, false);
        }

        protected override void Settings()
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

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            // Shouldnt update or draw as normal.
            if (!IsMenuVisible()) return;

            if (currentMenuState == MenuState.StartMenu)
            {
                DrawMenuText(spriteBatch, "Pause Menu", TextPos);
            }
            else
            {
                DrawMenuText(spriteBatch, "Settings", TextPos);
            }
        }

        private bool IsMenuVisible()
        {
            return StartMenuObjects[0].IsEnabled || PauseMenuObjects[0].IsEnabled;
        }
    }
}
