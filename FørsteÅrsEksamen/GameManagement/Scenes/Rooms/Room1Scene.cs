using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.Factory;
using DoctorsDungeon.Factory.Gui;
using DoctorsDungeon.LiteDB;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using DoctorsDungeon.CommandPattern.Commands;
using System.Timers;
using System;
using DoctorsDungeon.ComponentPattern.PlayerClasses;

namespace DoctorsDungeon.GameManagement.Scenes.Rooms;

// Oscar
public class Room1Scene : RoomBase
{

    /*
     * Could have made this into a class, so its easy to show and hide tutorials in all rooms.
     * Would need to have a way to either remove quest after it has been completed
     * Or being able to change into another tutorial text
     */
    private bool _hasRemovedMoveChecks, _hasRemovedAttackChecks, _hasRemovedDashChecks, _hasRemovedPauseChecks, _hasRemovedPotionChecks;
    private CustomCmd changeToAttackTutorialCmd, changeToDashTutorialCmd, changeToPauseTutorialCmd, completedBaseTutorialCmd, potionTutorialCmd;

    private string _tutorialText;
    private string _moveTutorialText = "To move: W A S D";
    private string _attackTutorialText = "Left click to attack";
    private string _dashTutorialText = "Space to Dash\nGain brief damage immunity";
    private string _pauseTutorialText = "Press ESC to pause\nDoes not pause game";
    private string _potionTutorialText = "Find a red potion\nPress E to use and heal";
    private string _finnishedBaseTutorialText = "Completed base tutorial"; 
    private string _finnishedPotionTutorialText = "Completed full tutorial\nHere is 100 gold"; 


    private bool _startRemoveTimer;
    private double _tutorialRemoveTimer, _tutorialHowLongOnScreen = 2f;

    public override void Initialize()
    {
        GridName = "Level1";
        GridWidth = 40;
        GridHeight = 28;

        SaveData.Level_Reached = 1;

        BackGroundTexture = TextureNames.Level1BG;
        ForeGroundTexture = TextureNames.Level1FG;

        base.Initialize();
        AddListenerCommands();
    }

    protected override void SetSpawnPotions()
    {
        PlayerSpawnPos = new Point(10, 3);
        EndPointSpawnPos = new Point(33, 2);

        EnemySpawnPoints = new() {
        new Point(10, 21),
        new Point(25, 21),
        new Point(37, 12),};

        PotionSpawnPoints = new() {
        new Point(7, 4),
        new Point(29, 9),};

        MiscGameObjectsInRoom = new()
        {
            { new Point(13, 5), TraningDummyFactory.Create()}
        };
    }

    private void AddListenerCommands()
    {
        if (SaveData.HasCompletedFullTutorial) return;

        _tutorialText = _moveTutorialText;

        changeToAttackTutorialCmd = new(ChangeToAttackTutorial);
        InputHandler.Instance.AddKeyUpdateCommand(DownMovementKey, changeToAttackTutorialCmd);
        InputHandler.Instance.AddKeyUpdateCommand(UpMovementKey, changeToAttackTutorialCmd);
        InputHandler.Instance.AddKeyUpdateCommand(LeftMovementKey, changeToAttackTutorialCmd);
        InputHandler.Instance.AddKeyUpdateCommand(RightMovementKey, changeToAttackTutorialCmd);

        // A bool for each check
        changeToDashTutorialCmd = new(ChangeToDashTutorial);
        InputHandler.Instance.AddMouseUpdateCommand(AttackSimpelAttackKey, changeToDashTutorialCmd);

        changeToPauseTutorialCmd = new(ChangeToPauseTutorial);
        InputHandler.Instance.AddKeyUpdateCommand(DashKey, changeToPauseTutorialCmd);

        completedBaseTutorialCmd = new(FinnishStartTutorial);
        InputHandler.Instance.AddKeyUpdateCommand(TogglePauseMenuKey, completedBaseTutorialCmd);


        // We need to have another way to start the potion tutorial, and we use a Action here to do that
        Health playerHealth = PlayerGo.GetComponent<Health>();
        playerHealth.On50Hp += ChangeTextToHealthReminder;

        potionTutorialCmd = new(FinnishedPotionTutorial);
        InputHandler.Instance.AddKeyUpdateCommand(UseItem, potionTutorialCmd);

    }
    private void ChangeToAttackTutorial()
    {
        if (_tutorialText != _moveTutorialText) return;
        _tutorialText = _attackTutorialText;
    }
    private void ChangeToDashTutorial()
    {
        if (_tutorialText != _attackTutorialText) return;
        _tutorialText = _dashTutorialText;
    }
    private void ChangeToPauseTutorial()
    {
        if (_tutorialText != _dashTutorialText) return;
        _tutorialText = _pauseTutorialText;
    }
    private void FinnishStartTutorial()
    {
        if (_tutorialText != _pauseTutorialText) return;
        _tutorialText = _finnishedBaseTutorialText;
        _startRemoveTimer = true;
    }

    // Gets set when player health is low
    private void ChangeTextToHealthReminder()
    {
        if (_tutorialText == "" || _tutorialText == _finnishedBaseTutorialText)
        {
            _startRemoveTimer = false;
            _tutorialRemoveTimer = 0f;
            _tutorialText = _potionTutorialText;
        }
    }
    private void FinnishedPotionTutorial()
    {
        if (!Player.HasUsedItem || _tutorialText != _potionTutorialText) return;
        _tutorialText = _finnishedPotionTutorialText;
        SaveData.HasCompletedFullTutorial = true;
        DB.Instance.SaveGame(SaveData.CurrentSaveID);
        _startRemoveTimer = true;
    }

    public override void Update()
    {
        base.Update();

        CheckIfShouldDeleteMoveChecks();
        CheckIfShouldDeleteAttackChecks();
        CheckIfShouldDeleteDashChecks();
        CheckIfShouldDeletePauseChecks();
        CheckIfShouldDeletePotionChecks();

        if (_startRemoveTimer)
            _tutorialRemoveTimer += GameWorld.DeltaTime;
    }
    
    #region Remove Commands
    private void CheckIfShouldDeleteMoveChecks()
    {
        if (_hasRemovedMoveChecks || _tutorialText != _dashTutorialText) return;
        
        _hasRemovedMoveChecks = true;
        InputHandler.Instance.RemoveKeyUpdateCommand(DownMovementKey, changeToAttackTutorialCmd);
        InputHandler.Instance.RemoveKeyUpdateCommand(UpMovementKey, changeToAttackTutorialCmd);
        InputHandler.Instance.RemoveKeyUpdateCommand(LeftMovementKey, changeToAttackTutorialCmd);
        InputHandler.Instance.RemoveKeyUpdateCommand(RightMovementKey, changeToAttackTutorialCmd);
    }

    private void CheckIfShouldDeleteAttackChecks()
    {
        if (_hasRemovedAttackChecks || _tutorialText != _dashTutorialText) return;
        _hasRemovedAttackChecks = true;
        InputHandler.Instance.RemoveMouseUpdateCommand(AttackSimpelAttackKey, changeToDashTutorialCmd);
    }

    private void CheckIfShouldDeleteDashChecks()
    {
        if (_hasRemovedDashChecks || _tutorialText != _pauseTutorialText) return;
        _hasRemovedDashChecks = true;
        InputHandler.Instance.RemoveKeyUpdateCommand(DashKey, changeToPauseTutorialCmd);
    }

    private void CheckIfShouldDeletePauseChecks()
    {
        if (_hasRemovedPauseChecks || _tutorialText != _finnishedBaseTutorialText) return;
        _hasRemovedPauseChecks = true;
        InputHandler.Instance.RemoveKeyUpdateCommand(TogglePauseMenuKey, completedBaseTutorialCmd);
    }

    // Remove potion check
    private void CheckIfShouldDeletePotionChecks()
    {
        if (_hasRemovedPotionChecks || _tutorialText != _finnishedBaseTutorialText) return;
        _hasRemovedPotionChecks = true;
        InputHandler.Instance.RemoveKeyUpdateCommand(UseItem, potionTutorialCmd);
    }
    #endregion

    // First WASD
    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        base.DrawOnScreen(spriteBatch);

        if (string.IsNullOrEmpty(_tutorialText) || SaveData.HasCompletedFullTutorial) return;

        if (_startRemoveTimer)
        {
            double normalizedTime = _tutorialRemoveTimer / _tutorialHowLongOnScreen;
            if (normalizedTime >= 1) return; // Dont draw if the timer is not 
        }

        Vector2 size = GlobalTextures.DefaultFont.MeasureString(_tutorialText);
        Vector2 textPos = GameWorld.Instance.UiCam.TopRight + new Vector2(-260, 205);

        SpriteRenderer.DrawCenteredSprite(spriteBatch, TextureNames.QuestUnder, textPos, BaseMath.TransitionColor(Color.White), LayerDepth.Default);

        GuiMethods.DrawTextCentered(spriteBatch, GlobalTextures.DefaultFont, textPos, _tutorialText, CurrentTextColor);
    }
}