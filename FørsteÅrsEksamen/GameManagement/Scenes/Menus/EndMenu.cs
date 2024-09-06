using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.GUI;
using ShamansDungeon.Factory.Gui;
using ShamansDungeon.LiteDB;
using Microsoft.Xna.Framework.Graphics;

namespace ShamansDungeon.GameManagement.Scenes.Menus;

// Asser
public class EndMenu : MenuScene
{
    protected override void InitFirstMenu()
    {
        GameObject startBtn = ButtonFactory.Create("New Run", true,
                        () => { GameWorld.Instance.ChangeScene(SceneNames.CharacterSelectorMenu); });
        FirstMenuObjects.Add(startBtn);

        GameObject settingsBtn = ButtonFactory.Create("Settings", true, ShowHideSecondMenu);
        FirstMenuObjects.Add(settingsBtn);

        GameObject mainMenuBtn = ButtonFactory.Create("Main Menu", true,
                () => { GameWorld.Instance.ChangeScene(SceneNames.MainMenu); });
        FirstMenuObjects.Add(mainMenuBtn);

        GameObject quitBtn = ButtonFactory.Create("Quit", true, GameWorld.Instance.Exit);
        FirstMenuObjects.Add(quitBtn);
    }

    protected override void InitSecondMenu()
    {
        GameObject musicVolGo = ButtonFactory.Create("", true, ChangeMusic, TextureNames.LongButton);
        MusicBtn = musicVolGo.GetComponent<Button>();
        MusicBtn.Text = $"Music Volume {GlobalSounds.MusicVolume * 100}%";
        SecondMenuObjects.Add(musicVolGo);

        GameObject sfxVolGo = ButtonFactory.Create("", true, ChangeSfx, TextureNames.LongButton);
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
            string text;
            if (SaveData.HasWon) text = "You Won!";
            else if (SaveData.LostByTime) text = "Time Ran Out";
            else text = "Try Again";

            DrawMenuText(spriteBatch, text, TextPos);
        }
    }
}