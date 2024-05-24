using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.Factory.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FørsteÅrsEksamen.LiteDB;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Menus
{
    public class EndMenu : MenuScene
    {

        protected override void InitFirstMenu()
        {
            GameObject startBtn = ButtonFactory.Create("New Run", true,
                            () => { GameWorld.Instance.ChangeScene(SceneNames.SaveFileMenu); });
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
                string text = SaveData.HasWon == true ? "You Won!" : "Try Again";
                DrawMenuText(spriteBatch, text, TextPos);
            }
        }
    }
}
