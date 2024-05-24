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
                            //() => { GameWorld.Instance.ChangeDungounScene(SceneNames.DungounRoom, 1); });
            //() => { GameWorld.Instance.ChangeScene(ScenesNames.OscarTestScene); });
            FirstMenuObjects.Add(startBtn);

            GameObject settingsBtn = ButtonFactory.Create("Settings", true, ShowHideSecondMenu);
            FirstMenuObjects.Add(settingsBtn);

            GameObject quitBtn = ButtonFactory.Create("Quit", true, GameWorld.Instance.Exit);
            FirstMenuObjects.Add(quitBtn);

            GuiMethods.PlaceGameObjectsVertical(FirstMenuObjects, TextPos + new Vector2(0, 75), 25);
        }

        protected override void InitSecondMenu()
        {
            GameObject musicVolGo = ButtonFactory.Create("", true, ChangeMusic);
            MusicBtn = musicVolGo.GetComponent<Button>();
            MusicBtn.ChangeScale(new Vector2(14, 4));
            MusicBtn.Text = $"Music Volume {GlobalSounds.MusicVolume * 100}%";
            SecondMenuObjects.Add(musicVolGo);

            GameObject sfxVolGo = ButtonFactory.Create("", true, ChangeSfx);
            SfxBtn = sfxVolGo.GetComponent<Button>();
            SfxBtn.ChangeScale(new Vector2(14, 4));
            SfxBtn.Text = $"SFX Volume {GlobalSounds.SfxVolume * 100}%";
            SecondMenuObjects.Add(sfxVolGo);

            GameObject quitBtn = ButtonFactory.Create("Back", true, ShowHideSecondMenu);
            SecondMenuObjects.Add(quitBtn);

            ShowHideGameObjects(SecondMenuObjects, false);

            GuiMethods.PlaceGameObjectsVertical(SecondMenuObjects, TextPos + new Vector2(0, 75), 25);
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