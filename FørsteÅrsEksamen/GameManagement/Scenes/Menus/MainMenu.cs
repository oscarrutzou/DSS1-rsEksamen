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
    public class MainMenu : MenuScene
    {
        public override void Initialize()
        {
            base.Initialize();
            // Draw BG
        }

        protected override void InitStartMenu()
        {
            GameObject startBtn = ButtonFactory.Create("Start Game", true,
                            () => { GameWorld.Instance.ChangeRoom(0); });
            //() => { GameWorld.Instance.ChangeScene(ScenesNames.OscarTestScene); });
            StartMenuObjects.Add(startBtn);

            GameObject settingsBtn = ButtonFactory.Create("Settings", true, Settings);
            StartMenuObjects.Add(settingsBtn);

            GameObject quitBtn = ButtonFactory.Create("Quit", true, GameWorld.Instance.Exit);
            StartMenuObjects.Add(quitBtn);

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

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);

            if (ShowSettings)
            {
                DrawMenuText(spriteBatch, "Settings", TextPos);
            }
            else
            {
                DrawMenuText(spriteBatch, "Doctor's Dunguon", TextPos);
            }
        }
    }
}