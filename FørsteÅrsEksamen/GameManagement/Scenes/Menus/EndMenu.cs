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
        }

        protected override void InitSecondMenu()
        {
            GameObject musicVolGo = ButtonFactory.Create("", true, ChangeMusic, TextureNames.LargeBtn);
            MusicBtn = musicVolGo.GetComponent<Button>();
            MusicBtn.Text = $"Music Volume {GlobalSounds.MusicVolume * 100}%";
            SecondMenuObjects.Add(musicVolGo);

            GameObject sfxVolGo = ButtonFactory.Create("", true, ChangeSfx, TextureNames.LargeBtn);
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
                //string text = SaveData.HasWon == true ? "You Won!" : "Try Again";
                string text;
                if (SaveData.HasWon) text = "You Won!";
                else if (SaveData.LostByTime) text = "Time Ran Out";
                else text = "Try Again";

                DrawMenuText(spriteBatch, text, TextPos);
            }
        }
    }
}
