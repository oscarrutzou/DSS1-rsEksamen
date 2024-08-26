using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DoctorsDungeon.GameManagement.Scenes.Menus;

// Oscar
public abstract class MenuScene : Scene
{
    protected bool ShowSecondMenu;

    protected List<GameObject> FirstMenuObjects;
    protected List<GameObject> SecondMenuObjects;

    protected SpriteFont Font;
    protected Vector2 TextPos;
    protected Button MusicBtn, SfxBtn;

    public override void Initialize()
    {
        SaveData.ResetPlayer();

        FirstMenuObjects = new();
        SecondMenuObjects = new();
        ShowSecondMenu = false;

        //GlobalSounds.PlayMenuMusic = true;
        GameWorld.Instance.ShowBG = true;

        Font = GlobalTextures.BigFont;
        TextPos = GameWorld.Instance.UiCam.Center + new Vector2(0, -200);

        OnFirstCleanUp = AfterFirstCleanUp;

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

    protected virtual void InitFirstMenu()
    { }

    protected virtual void InitSecondMenu()
    { }

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
        // If the scene is changing, then it should use the lerp of the color. Already have the start color
        GuiMethods.DrawTextCentered(spriteBatch, Font, position, text, CurrentTextColor);
    }
}