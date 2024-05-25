using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Menus
{
    public abstract class MenuScene : Scene
    {
        protected bool ShowSecondMenu;

        protected List<GameObject> FirstMenuObjects;
        protected List<GameObject> SecondMenuObjects;

        protected SpriteFont Font;
        protected Vector2 TextPos;
        protected Button MusicBtn, SfxBtn;
        protected bool ShowBG = true;
        public override void Initialize()
        {
            FirstMenuObjects = new();
            SecondMenuObjects = new();
            ShowSecondMenu = false;

            GlobalSounds.InMenu = true;

            Font = GlobalTextures.BigFont;
            TextPos = GameWorld.Instance.UiCam.Center + new Vector2(0, -200);
            
            OnFirstCleanUp = AfterFirstCleanUp;

            SpawnBG();
            InitFirstMenu();
            InitSecondMenu();

            foreach (GameObject firstObj in FirstMenuObjects)
            {
                GameWorld.Instance.Instantiate(firstObj);
            }

            foreach (GameObject secondObj in SecondMenuObjects)
            {
                GameWorld.Instance.Instantiate(secondObj);
            }
        }

        protected virtual void InitFirstMenu() { }
        protected virtual void InitSecondMenu() { }
        private void SpawnBG()
        {
            if (!ShowBG) return;
            GameObject go = new();
            go.Transform.Scale = new(4, 4);
            go.Type = GameObjectTypes.Background;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.SetLayerDepth(LayerDepth.WorldBackground);
            sr.SetSprite(TextureNames.SpaceBG1);
            GameWorld.Instance.Instantiate(go);
        }

        public virtual void AfterFirstCleanUp()
        {
            GuiMethods.PlaceGameObjectsVertical(FirstMenuObjects, TextPos + new Vector2(0, 75), 25);
            GuiMethods.PlaceGameObjectsVertical(SecondMenuObjects, TextPos + new Vector2(0, 75), 25);
        }

        protected virtual void ShowHideSecondMenu()
        {
            ShowSecondMenu = !ShowSecondMenu;

            if (ShowSecondMenu)
            {
                ShowHideGameObjects(FirstMenuObjects, false);
                ShowHideGameObjects(SecondMenuObjects, true);
            }
            else
            {
                ShowHideGameObjects(FirstMenuObjects, true);
                ShowHideGameObjects(SecondMenuObjects, false);
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
            GuiMethods.DrawTextCentered(spriteBatch, Font, position, text, Color.RoyalBlue);
        }
    }
}
