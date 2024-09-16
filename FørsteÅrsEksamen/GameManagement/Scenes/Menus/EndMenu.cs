using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.GUI;
using ShamansDungeon.Factory.Gui;
using ShamansDungeon.LiteDB;
using Microsoft.Xna.Framework.Graphics;
using System.Numerics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Linq;

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
            SetHighscore();

            return;
        }
        else if (SaveData.LostByTime) _menuText = "Time Ran Out";
        else _menuText = "Try Again...";

        highscoreList = DB.Instance.GetHighScores();
        List<HighscoreUserData> tempList = highscoreList.OrderByDescending(x => x.TimeLeft).ToList();
        highscoreList = tempList;

        GlobalSounds.PlaySound(SoundNames.LostGame, 1, 0.7f);
    }

    private void SetHighscore()
    {
        Random rnd = new();
        HighscoreUserData newData = new HighscoreUserData()
        {
            TimeLeft = SaveData.Time_Left,
            UserName = $"{SaveData.SelectedClass} nr {rnd.Next(0, 300)}",
        };
        highscoreList = DB.Instance.UpdateHighScore(newData);
        List<HighscoreUserData> tempList = highscoreList.OrderByDescending(x => x.TimeLeft).ToList();
        highscoreList = tempList;
    }

    List<HighscoreUserData> highscoreList;

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

        DrawHighScoreList(spriteBatch);
    }

    private void DrawHighScoreList(SpriteBatch spriteBatch)
    {
        if (highscoreList == null) return;
        Microsoft.Xna.Framework.Vector2 pos = GameWorld.Instance.UiCam.TopRight + new Microsoft.Xna.Framework.Vector2(-600, 200);
        spriteBatch.DrawString(GlobalTextures.BigFont, "HighScores", pos, CurrentTextColor);
        pos += new Microsoft.Xna.Framework.Vector2(0, 60);


        foreach (HighscoreUserData highscore in highscoreList)
        {
            pos += new Microsoft.Xna.Framework.Vector2(0, 20);
            
            TimeSpan time = TimeSpan.FromSeconds(highscore.TimeLeft);
            string timeLeft = $"Time Left: {time.Minutes:D2}:{time.Seconds:D2}";

            string data = $"{highscore.UserName}: {timeLeft}";
            spriteBatch.DrawString(GlobalTextures.DefaultFont, data, pos, CurrentTextColor);
        }
    }
}