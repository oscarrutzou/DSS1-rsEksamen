using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Menus
{
    public abstract class MenuScene : Scene
    {
        protected bool ShowSettings;

        protected List<GameObject> StartMenuObjects;
        protected List<GameObject> PauseMenuObjects;

        protected SpriteFont Font;
        protected Vector2 TextPos;
        protected Button MusicBtn, SfxBtn;

        public override void Initialize()
        {
            StartMenuObjects = new();
            PauseMenuObjects = new();
            ShowSettings = false;

            GlobalSounds.InMenu = true;

            Font = GlobalTextures.BigFont;
            TextPos = GameWorld.Instance.UiCam.Center + new Vector2(0, -200);

            InitStartMenu();
            InitSettingsMenu();
        }
        protected virtual void InitStartMenu() { }
        protected virtual void InitSettingsMenu() { }
        protected virtual void Settings()
        {
            ShowSettings = !ShowSettings;

            if (ShowSettings)
            {
                ShowHideGameObjects(StartMenuObjects, false);
                ShowHideGameObjects(PauseMenuObjects, true);
            }
            else
            {
                ShowHideGameObjects(StartMenuObjects, true);
                ShowHideGameObjects(PauseMenuObjects, false);
            }
        }
        protected void ShowHideGameObjects(List<GameObject> gameObjects, bool isEnabled)
        {
            foreach (GameObject item in gameObjects)
            {
                item.IsEnabled = isEnabled;
            }
        }
        protected void ChangeMusic()
        {
            GlobalSounds.ChangeMusicVolume();
            MusicBtn.Text = $"Music Volume {GlobalSounds.MusicVolume * 100}%";
        }
        protected void ChangeSfx()
        {
            GlobalSounds.ChangeSfxVolume();
            SfxBtn.Text = $"SFX Volume {GlobalSounds.SfxVolume * 100}%";
        }
        protected void DrawMenuText(SpriteBatch spriteBatch, string text, Vector2 position)
        {
            GuiMethods.DrawTextCentered(spriteBatch, Font, position, text, Color.Black);
        }
    }
}
