using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.GUI;
using ShamansDungeon.Factory.Gui;
using Microsoft.Xna.Framework.Graphics;

namespace ShamansDungeon.GameManagement.Scenes.Menus;

// Stefan
public class PauseMenu : MenuScene
{
    private bool _isMenuVisible;

    private enum MenuState
    { StartMenu, SettingsMenu }

    private MenuState _currentMenuState = MenuState.StartMenu;

    public override void Initialize()
    {
        base.Initialize();
        //GameWorld.Instance.ShowBG = false;
        //GlobalSounds.InMenu = false; //Uncomment this when there has been made a ease in and out from menu and game music
    }

    protected override void InitFirstMenu()
    {
        GameObject startBtn = ButtonFactory.Create("Pause", true, TogglePauseMenu); // Delete?
        FirstMenuObjects.Add(startBtn);

        GameObject settingsBtn = ButtonFactory.Create("Settings", true, ShowHideSecondMenu);
        FirstMenuObjects.Add(settingsBtn);

        GameObject mainMenu = ButtonFactory.Create("Main Menu", true,
            () => { GameWorld.Instance.ChangeScene(SceneNames.MainMenu); });
        FirstMenuObjects.Add(mainMenu);

        GameObject quitBtn = ButtonFactory.Create("Quit", true, GameWorld.Instance.Exit);
        FirstMenuObjects.Add(quitBtn); 

        ShowHideGameObjects(FirstMenuObjects, false);
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

    public void TogglePauseMenu()
    {
        _isMenuVisible = !_isMenuVisible;
        GameWorld.IsPaused = _isMenuVisible;
        if (_isMenuVisible)
        {
            ShowMenu();
        }
        else
        {
            HideMenu();
        }
    }

    private void ShowMenu()
    {
        if (_currentMenuState == MenuState.StartMenu)
        {
            ShowHideGameObjects(FirstMenuObjects, true);
            ShowHideGameObjects(SecondMenuObjects, false);
        }
        else
        {
            ShowHideGameObjects(FirstMenuObjects, false);
            ShowHideGameObjects(SecondMenuObjects, true);
        }
    }

    private void HideMenu()
    {
        _currentMenuState = MenuState.StartMenu;
        ShowHideGameObjects(FirstMenuObjects, false);
        ShowHideGameObjects(SecondMenuObjects, false);
    }

    protected override void ShowHideSecondMenu()
    {
        if (_currentMenuState == MenuState.StartMenu)
        {
            _currentMenuState = MenuState.SettingsMenu;
        }
        else
        {
            _currentMenuState = MenuState.StartMenu;
        }
        ShowMenu();
    }

    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        // Shouldnt update or draw as normal.
        if (!IsMenuVisible()) return;

        //GameWorld.Instance.IsMouseVisible = true;

        if (_currentMenuState == MenuState.StartMenu)
        {
            DrawMenuText(spriteBatch, "Pause Menu", TextPos);
        }
        else
        {
            DrawMenuText(spriteBatch, "Settings", TextPos);
        }
    }

    private bool IsMenuVisible()
    {
        return FirstMenuObjects[0].IsEnabled || SecondMenuObjects[0].IsEnabled;
    }
}