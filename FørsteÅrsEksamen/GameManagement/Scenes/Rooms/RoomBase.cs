using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using DoctorsDungeon.CommandPattern.Commands;
using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Enemies;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.Factory;
using DoctorsDungeon.GameManagement.Scenes.Menus;
using DoctorsDungeon.LiteDB;
using DoctorsDungeon.Other;
using DoctorsDungeon.Factory.Gui;
using DoctorsDungeon.ComponentPattern.GUI;

namespace DoctorsDungeon.GameManagement.Scenes.Rooms;

// Oscar
public abstract class RoomBase : Scene
{
    #region Properties

    private PauseMenu _pauseMenu;

    protected string GridName;
    protected int GridWidth, GridHeight;
    protected TextureNames BackGroundTexture = TextureNames.TestLevelBG;
    protected TextureNames ForeGroundTexture = TextureNames.TestLevelFG;

    public Point PlayerSpawnPos, EndPointSpawnPos;
    protected GameObject PlayerGo;
    private Player _player;
    private Health _playerHealth;

    protected List<Point> EnemySpawnPoints = new();
    protected List<Point> PotionSpawnPoints = new();
    protected Dictionary<Point, GameObject> MiscGameObjectsInRoom { get; set; } = new();

    private TransferDoor _transferDoor;
    private SpriteRenderer _transferDoorSpriteRenderer;
    public List<Enemy> EnemiesInRoom { get; set; } = new();
    private List<Enemy> _aliveEnemies;

    private Spawner _spawner;

    private List<GameObject> _cells = new(); // For debug
    private Vector2 _startLeftPos;

    //private string startFinalText; // Used to set the start size of text for the hour glass,
    // so it dosent move when the timer counts down.
    #endregion Properties

    public override void Initialize()
    {

        SetSpawnPotions();

        // There needs to have been set some stuff before this base.Initialize (Look at Room1 for reference)
        PlayerGo = null; //Remove this from normal Scene and make another scene that sets all up.

        _pauseMenu = new PauseMenu();
        _pauseMenu.Initialize();
        OnFirstCleanUp = _pauseMenu.AfterFirstCleanUp; // We need to couple the pausemenu to the current RoomScene Action.

        SpawnTexture(BackGroundTexture, LayerDepth.WorldBackground);
        SpawnTexture(ForeGroundTexture, LayerDepth.WorldForeground);

        SpawnGrid();

        SpawnAndLoadPlayer();

        SpawnEndPos();

        SpawnHealthBar();
        SpawnEnemies();
        SpawnPotions();
        CenterMiscItems();

        SetStartTimeLeft();

        SetCommands();

        GridManager.Instance.SetCellsVisibility();
    }

    #region Initialize Methods

    private GameObject hourGlassIcon, inventoryIcon;
    private void SetStartTimeLeft()
    {
        _startLeftPos = GameWorld.Instance.UiCam.TopLeft + new Vector2(80, 130);
        inventoryIcon = IconFactory.CreateBackpackIcon();
        inventoryIcon.Transform.Position = _startLeftPos;

        GameWorld.Instance.Instantiate(inventoryIcon);
        _startLeftPos.X += 50f;

        hourGlassIcon = IconFactory.CreateHourGlassIcon();
        hourGlassIcon.Transform.Position = _startLeftPos;

        GameWorld.Instance.Instantiate(hourGlassIcon);

    }

    protected abstract void SetSpawnPotions();

    private void SpawnTexture(TextureNames textureName, LayerDepth layerDepth)
    {
        GameObject backgroundGo = new()
        {
            Type = GameObjectTypes.Background
        };

        SpriteRenderer spriteRenderer = backgroundGo.AddComponent<SpriteRenderer>();
        spriteRenderer.SetSprite(textureName);
        spriteRenderer.SetLayerDepth(layerDepth);
        spriteRenderer.IsCentered = false;

        GameWorld.Instance.Instantiate(backgroundGo);
    }

    private void SpawnGrid()
    {
        GameObject gridGo = new();
        Grid grid = gridGo.AddComponent<Grid>(GridName, new Vector2(0, 0), GridWidth, GridHeight);
        grid.GenerateGrid();
        GridManager.Instance.SaveLoadGrid(grid);
    }

    private void SpawnAndLoadPlayer()
    {
        DB.Instance.UpdateLoadRun(SaveData.CurrentSaveID);

        PlayerGo = SaveData.Player.GameObject;

        PlayerGo.Transform.Position = GridManager.Instance.CurrentGrid.Cells[PlayerSpawnPos].Transform.Position;
        PlayerGo.Transform.GridPosition = PlayerSpawnPos;
        GameWorld.Instance.WorldCam.Position = PlayerGo.Transform.Position;

        if (IndependentBackground.BackgroundEmitter == null) return;
        IndependentBackground.BackgroundEmitter.FollowGameObject(PlayerGo, Vector2.Zero);
    }

    private void SpawnHealthBar()
    {
        GameObject go = ScalableBarFactory.CreateHealthBar(PlayerGo, true);
        GameWorld.Instance.Instantiate(go);
    }


    private void SpawnEndPos()
    {
        GameObject endDoor = TransferDoorFactory.Create();
        _transferDoor = endDoor.GetComponent<TransferDoor>();
        _transferDoorSpriteRenderer = endDoor.GetComponent<SpriteRenderer>();
        endDoor.Transform.Position = GridManager.Instance.GetCornerPositionOfCell(EndPointSpawnPos);
        GameWorld.Instance.Instantiate(endDoor);
    }

    private void SpawnEnemies()
    {
        GameObject spawnerGo = new();
        _spawner = spawnerGo.AddComponent<Spawner>();
        EnemiesInRoom = _spawner.SpawnEnemies(EnemySpawnPoints, PlayerGo);
    }

    private void SpawnPotions()
    {
        _spawner.SpawnPotions(PotionSpawnPoints, PlayerGo);
    }

    private void CenterMiscItems()
    {
        foreach (Point point in MiscGameObjectsInRoom.Keys)
        {
            GameObject go = MiscGameObjectsInRoom[point];
            go.Transform.Position = GridManager.Instance.CurrentGrid.GetCellFromPoint(point).GameObject.Transform.Position;
        }
    }

    private void SetCommands()
    {
        _player = PlayerGo.GetComponent<Player>();
        _playerHealth = PlayerGo.GetComponent<Health>();
        InputHandler.Instance.AddKeyUpdateCommand(Keys.D, new MoveCmd(_player, new Vector2(1, 0)));
        InputHandler.Instance.AddKeyUpdateCommand(Keys.A, new MoveCmd(_player, new Vector2(-1, 0)));
        InputHandler.Instance.AddKeyUpdateCommand(Keys.W, new MoveCmd(_player, new Vector2(0, -1)));
        InputHandler.Instance.AddKeyUpdateCommand(Keys.S, new MoveCmd(_player, new Vector2(0, 1)));

        InputHandler.Instance.AddMouseUpdateCommand(MouseCmdState.Left, new CustomCmd(_player.Attack));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(_pauseMenu.TogglePauseMenu));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.E, new CustomCmd(_player.UseItem));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space, new CustomCmd(_player.ResetDash));

        // For debugging
        if (!GameWorld.DebugAndCheats) return;

        //InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space, new CustomCmd(_player.Attack));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Enter, new CustomCmd(ChangeScene));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.O, new CustomCmd(() => { DB.Instance.SaveGrid(GridManager.Instance.CurrentGrid); }));

        //InputHandler.Instance.AddKeyButtonDownCommand(Keys.Q, new CustomCmd(() => { player.GameObject.GetComponent<Health>().TakeDamage(rnd.Next(500000000, 500000000)); }));
    }

    private void ChangeScene()
    {
        int newRoomNr = SaveData.Level_Reached + 1;
        GameWorld.Instance.ChangeDungeonScene(SceneNames.DungeonRoom, newRoomNr);
    }

    #endregion Initialize Methods

    public override void Update()
    {
        SaveData.Time_Left -= GameWorld.DeltaTime;

        if (SaveData.Time_Left <= 0) // Player ran out of Time
        {
            SaveData.Time_Left = 0;
            SaveData.LostByTime = true;
            // Makes the blood spray up, could make it positive or negative depending of the direction
            _playerHealth.TakeDamage(1000, _player.GameObject.Transform.Position + new Vector2(30, -30)); // Kills the player
        }

        // Check if enemies has been killed
        _aliveEnemies = EnemiesInRoom.Where(x => x.State != CharacterState.Dead).ToList();

        if (_aliveEnemies.Count == 0) // All enemies are dead to
        {
            OnAllEnemiesDied();
        }

        base.Update();
    }
     
    private void OnAllEnemiesDied()
    {
        // The transferDoor.emitter == null is there for when there is 0 enemies in the room at the start
        if (_transferDoorSpriteRenderer.ShouldDrawSprite == false || _transferDoor.emitter == null) return; // To stop method from being run twice.

        _transferDoorSpriteRenderer.ShouldDrawSprite = false;
        _transferDoor.CanTranser = true;
        _transferDoor.emitter.PlayEmitter();
    }

    #region Draw

    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        base.DrawOnScreen(spriteBatch);

        _pauseMenu.DrawOnScreen(spriteBatch);

        DrawTimer(spriteBatch, _startLeftPos);

        DrawQuest(spriteBatch);

        if (!InputHandler.Instance.DebugMode) return;
        DebugDraw(spriteBatch);
    }

    private void DrawQuest(SpriteBatch spriteBatch)
    {
        _aliveEnemies = EnemiesInRoom.Where(x => x.State != CharacterState.Dead).ToList();

        int dead = EnemiesInRoom.Count - _aliveEnemies.Count;
        int amountToKill = EnemiesInRoom.Count - dead;

        string text = $"Enemies left {amountToKill}/{EnemiesInRoom.Count}";//

        Vector2 size = GlobalTextures.DefaultFont.MeasureString(text);
        Vector2 textPos = GameWorld.Instance.UiCam.TopRight + new Vector2(-260, 55);

        Color questUnderColor = Color.White;
        if (IsChangingScene)
            questUnderColor = Color.Lerp(Color.White, Color.Transparent, (float)NormalizedTransitionProgress);

        SpriteRenderer.DrawCenteredSprite(spriteBatch, TextureNames.QuestUnder, textPos, questUnderColor, LayerDepth.Default);

        GuiMethods.DrawTextCentered(spriteBatch, GlobalTextures.DefaultFont, textPos, text, CurrentTextColor);
    }

    private void DrawTimer(SpriteBatch spriteBatch, Vector2 timerPos)
    {
        TimeSpan time = TimeSpan.FromSeconds(SaveData.Time_Left);
        string finalText = $"Time Left: {time.Minutes:D2}:{time.Seconds:D2}";

        Vector2 lineSize = GlobalTextures.DefaultFont.MeasureString(finalText);
        Vector2 center = timerPos - new Vector2(0, lineSize.Y / 2);

        spriteBatch.DrawString(GlobalTextures.DefaultFont, finalText, center, CurrentTextColor);
    }

    private void DebugDraw(SpriteBatch spriteBatch)
    {
        Vector2 startPos = GameWorld.Instance.UiCam.LeftCenter;
        
        Vector2 mousePos = InputHandler.Instance.MouseOnUI;

        Vector2 offset = new(0, 30);

        DrawString(spriteBatch, $"MousePos UI {mousePos}", startPos);

        GameObject cellGo = GridManager.Instance.GetCellAtPos(InputHandler.Instance.MouseInWorld);
        if (cellGo != null)
        {
            startPos += offset;
            Point cellGridPos = cellGo.Transform.GridPosition;
            DrawString(spriteBatch, $"Cell Point from MousePos: {cellGridPos}", startPos);
        }
        startPos += offset;
        DrawString(spriteBatch, $"PlayerPos {PlayerGo.Transform.Position}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Player Room Nr {_player.CollisionNr}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Cell GameObjects in scene {_cells.Count}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Current Level Reached {SaveData.Level_Reached}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Grid Current Draw {GridManager.Instance.CurrentDrawSelected}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Grid Collision Nr {GridManager.Instance.ColliderNrIndex}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Grid Room Nr {GridManager.Instance.RoomNrIndex}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"player.velocity {_player.velocity}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Player totalMovementInput: {_player.totalMovementInput.X},{_player.totalMovementInput.Y}", startPos);
    }

    protected void DrawString(SpriteBatch spriteBatch, string text, Vector2 position)
    {
        spriteBatch.DrawString(GlobalTextures.DefaultFont, text, position, Color.Pink, 0f, Vector2.Zero, 1, SpriteEffects.None, SpriteRenderer.GetLayerDepth(LayerDepth.Text));
    }

    #endregion Draw
}