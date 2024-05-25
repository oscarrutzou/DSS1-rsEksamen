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

        protected override void InitFirstMenu()
        {
            GameObject startBtn = ButtonFactory.Create("Save Files", true,
                            () => { GameWorld.Instance.ChangeScene(SceneNames.SaveFileMenu); });

            FirstMenuObjects.Add(startBtn);

            GameObject settingsBtn = ButtonFactory.Create("Settings", true, ShowHideSecondMenu);
            FirstMenuObjects.Add(settingsBtn);

            GameObject quitBtn = ButtonFactory.Create("Quit", true, GameWorld.Instance.Exit);
            FirstMenuObjects.Add(quitBtn);
        }

        protected override void InitSecondMenu()
        {
            GameObject musicVolGo = ButtonFactory.Create("", true, ChangeMusic, TextureNames.LargeBtn, AnimNames.LargeBtn);
            MusicBtn = musicVolGo.GetComponent<Button>();
            MusicBtn.Text = $"Music Volume {GlobalSounds.MusicVolume * 100}%";
            SecondMenuObjects.Add(musicVolGo);

            GameObject sfxVolGo = ButtonFactory.Create("", true, ChangeSfx,TextureNames.LargeBtn, AnimNames.LargeBtn);
            SfxBtn = sfxVolGo.GetComponent<Button>();
            SfxBtn.Text = $"SFX Volume {GlobalSounds.SfxVolume * 100}%";
            SecondMenuObjects.Add(sfxVolGo);

            GameObject quitBtn = ButtonFactory.Create("Back", true, ShowHideSecondMenu);
            SecondMenuObjects.Add(quitBtn);

            ShowHideGameObjects(SecondMenuObjects, false);
        }

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);

            if (ShowSecondMenu)
            {
                DrawMenuText(spriteBatch, "Settings", TextPos);
            }
            else
            {
                DrawMenuText(spriteBatch, "Doctor's Dungeon", TextPos);
            }
        }
    }
}