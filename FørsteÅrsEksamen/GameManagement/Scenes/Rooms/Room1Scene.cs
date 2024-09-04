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

namespace DoctorsDungeon.GameManagement.Scenes.Rooms;

// Oscar
public class Room1Scene : RoomBase
{

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
        _textPos = GameWorld.Instance.UiCam.LeftCenter + new Vector2(30, -40);
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
    private bool _hasRemovedMoveChecks, _hasRemovedAttackChecks, _hasRemovedDashChecks, _hasRemovedPauseChecks;
    private CustomCmd changeToAttackTutorialCmd, changeToDashTutorialCmd, changeToPauseTutorialCmd, completedTutorialCmd;

    private string _tutorialText;
    private string _moveTutorialText = "To move: W\n" +
                                   "        A S D";
    private string _attackTutorialText = "Left click to attack";
    private string _dashTutorialText = "Space to Dash\nGain brief damage immunity";
    private string _pauseTutorialText = "Press ESC to show pause menu \nDoes not pause game";
    private string _finnishedTutorialText = "Completed tutorial";
    private Vector2 _textPos;
    // ADd a listener to the keys
    // If command is used, change the tutorial to the other strings its missing.

    private void AddListenerCommands()
    {
        _tutorialText = _moveTutorialText;

        changeToAttackTutorialCmd = new(ChangeToAttackTutorial);
        InputHandler.Instance.AddKeyButtonDownCommand(DownMovementKey, changeToAttackTutorialCmd);
        InputHandler.Instance.AddKeyButtonDownCommand(UpMovementKey, changeToAttackTutorialCmd);
        InputHandler.Instance.AddKeyButtonDownCommand(LeftMovementKey, changeToAttackTutorialCmd);
        InputHandler.Instance.AddKeyButtonDownCommand(RightMovementKey, changeToAttackTutorialCmd);

        // A bool for each check
        changeToDashTutorialCmd = new(ChangeToDashTutorial);
        InputHandler.Instance.AddMouseButtonDownCommand(AttackSimpelAttackKey, changeToDashTutorialCmd);

        changeToPauseTutorialCmd = new(ChangeToPauseTutorial);
        InputHandler.Instance.AddKeyButtonDownCommand(DashKey, changeToPauseTutorialCmd);

        completedTutorialCmd = new(FinnishStartTutorial);
        InputHandler.Instance.AddKeyButtonDownCommand(TogglePauseMenuKey, completedTutorialCmd);
    }

    // wait a bit? 
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
        _tutorialText = _finnishedTutorialText;
    }

    public override void Update()
    {
        base.Update();

        CheckIfShouldDeleteMoveChecks();
        CheckIfShouldDeleteAttackChecks();
        CheckIfShouldDeleteDashChecks();
        CheckIfShouldDeletePauseChecks();
    }

    private void CheckIfShouldDeleteMoveChecks()
    {
        if (_hasRemovedMoveChecks || _tutorialText != _dashTutorialText) return;
        
        _hasRemovedMoveChecks = true;
        InputHandler.Instance.RemoveKeyButtonDownCommand(DownMovementKey, changeToAttackTutorialCmd);
        InputHandler.Instance.RemoveKeyButtonDownCommand(UpMovementKey, changeToAttackTutorialCmd);
        InputHandler.Instance.RemoveKeyButtonDownCommand(LeftMovementKey, changeToAttackTutorialCmd);
        InputHandler.Instance.RemoveKeyButtonDownCommand(RightMovementKey, changeToAttackTutorialCmd);
    }

    private void CheckIfShouldDeleteAttackChecks()
    {
        if (_hasRemovedAttackChecks || _tutorialText != _dashTutorialText) return;

        _hasRemovedAttackChecks = true;
        InputHandler.Instance.RemoveMouseButtonDownCommand(AttackSimpelAttackKey, changeToDashTutorialCmd);
    }

    private void CheckIfShouldDeleteDashChecks()
    {
        if (_hasRemovedDashChecks || _tutorialText != _pauseTutorialText) return;

        _hasRemovedDashChecks = true;
        InputHandler.Instance.RemoveKeyButtonDownCommand(DashKey, changeToPauseTutorialCmd);
    }


    private void CheckIfShouldDeletePauseChecks()
    {
        if (_hasRemovedPauseChecks || _tutorialText != _finnishedTutorialText) return;

        _hasRemovedPauseChecks = true;
        InputHandler.Instance.RemoveKeyButtonDownCommand(TogglePauseMenuKey, completedTutorialCmd);
    }


    // First WASD
    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        base.DrawOnScreen(spriteBatch);

        if (string.IsNullOrEmpty(_tutorialText)) return;

        spriteBatch.DrawString(GlobalTextures.DefaultFont, _tutorialText, _textPos, CurrentTextColor);
    }
}