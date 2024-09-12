using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.GUI;
using ShamansDungeon.Factory.Gui;
using ShamansDungeon.LiteDB;
using Microsoft.Xna.Framework.Graphics;
using static System.Net.Mime.MediaTypeNames;

namespace ShamansDungeon.GameManagement.Scenes.Menus;

// Asser
public class EndMenu : MenuScene
{
    protected override void InitFirstMenu()
    {
        GameObject startBtn = ButtonFactory.Create("New Run", true, SetCharacterSelectorMenu);
        FirstMenuObjects.Add(startBtn);

        GameObject settingsBtn = ButtonFactory.Create("Settings", true, ShowHideSecondMenu);
        FirstMenuObjects.Add(settingsBtn);

        GameObject mainMenuBtn = ButtonFactory.Create("Main Menu", true, SetMainMenu);
        FirstMenuObjects.Add(mainMenuBtn);

        GameObject quitBtn = ButtonFactory.Create("Quit", true, GameWorld.Instance.Exit);
        FirstMenuObjects.Add(quitBtn);

        SetMenuText();
    }
    private void SetMainMenu()
    {
        SaveData.SetBaseValues();
        GameWorld.Instance.ChangeScene(SceneNames.MainMenu);
    }
    private void SetCharacterSelectorMenu()
    {
        SaveData.SetBaseValues(); 
        DB.Instance.LoadGame(); // Already have deleted run save
        GameWorld.Instance.ChangeScene(SceneNames.CharacterSelectorMenu);
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
    private string _menuText;
    private void SetMenuText()
    {
        if (SaveData.HasWon)
        {
            GlobalSounds.PlaySound(SoundNames.WinGame, 1, 1);
            _menuText = "You Won!";
            return;
        }
        else if (SaveData.LostByTime) _menuText = "Time Ran Out";
        else _menuText = "Try Again...";

        GlobalSounds.PlaySound(SoundNames.LostGame, 1, 0.7f);
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
            DrawMenuText(spriteBatch, _menuText, TextPos);
        }
    }
}