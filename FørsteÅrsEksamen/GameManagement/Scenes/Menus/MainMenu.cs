using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.Factory.Gui;
using Microsoft.Xna.Framework.Graphics;

namespace DoctorsDungeon.GameManagement.Scenes.Menus
{
    // Oscar
    public class MainMenu : MenuScene
    {
        protected override void InitFirstMenu()
        {
            GameObject startBtn = ButtonFactory.Create("Play", true,
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
                DrawMenuText(spriteBatch, "Doctor's Dungeon", TextPos);
            }
        }
    }
}