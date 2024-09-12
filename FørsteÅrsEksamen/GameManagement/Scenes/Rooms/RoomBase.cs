using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShamansDungeon.CommandPattern.Commands;
using ShamansDungeon.CommandPattern;
using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.Enemies;
using ShamansDungeon.ComponentPattern.Path;
using ShamansDungeon.ComponentPattern.PlayerClasses;
using ShamansDungeon.ComponentPattern.WorldObjects;
using ShamansDungeon.Factory;
using ShamansDungeon.GameManagement.Scenes.Menus;
using ShamansDungeon.LiteDB;
using ShamansDungeon.Other;
using ShamansDungeon.Factory.Gui;
using ShamansDungeon.ComponentPattern.GUI;
using ShamansDungeon.ComponentPattern.WorldObjects.PickUps;

namespace ShamansDungeon.GameManagement.Scenes.Rooms;

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
    protected Player Player;
    private Health _playerHealth;

    protected List<Point> EnemySpawnPoints = new();
    protected List<Point> PotionSpawnPoints = new();
    protected Dictionary<Point, GameObject> MiscGameObjectsInRoom { get; set; } = new();

    private TransferDoor _transferDoor;
    private SpriteRenderer _transferDoorSpriteRenderer;
    public List<Enemy> EnemiesInRoom { get; set; } = new();
    private List<Enemy> _aliveEnemies;
    protected Spawner RoomSpawner;
    
    protected string QuestText;

    public static MouseCmdState AttackSimpelAttackKey = MouseCmdState.Left;
    public static Keys UseItem = Keys.E;
    public static Keys DashKey = Keys.Space;
    public static Keys TogglePauseMenuKey = Keys.Escape;
    public static Keys ToggleStatsMenuKey = Keys.Tab;
    public static Keys LeftMovementKey = Keys.A, RightMovementKey = Keys.D, UpMovementKey = Keys.W, DownMovementKey = Keys.S;

    private List<GameObject> _cells = new(); // For debug
    private Vector2 _startLeftPos;
    private GameObject hourGlassIcon, inventoryIcon;
    private bool _showStats;
    #endregion Properties

    public override void Initialize()
    {
        SetSpawnPotions();

        // There needs to have been set some stuff before this base.Initialize (Look at Room1 for reference)
        PlayerGo = null; //Remove this from normal Scene and make another scene that sets all up.
        _showStats = false;
        _pauseMenu = new PauseMenu();
        _pauseMenu.Initialize();
        OnFirstCleanUp = _pauseMenu.AfterFirstCleanUp; // We need to couple the pausemenu to the current RoomScene Action.

        SpawnTexture(BackGroundTexture, LayerDepth.WorldBackground);
        SpawnTexture(ForeGroundTexture, LayerDepth.WorldForeground);

        SpawnGrid();

        SpawnAndLoadPlayer();

        SpawnEndPos();
        SpawnTutorialBox();
        SpawnHealthBar();
        SpawnEnemies();
        SpawnPotions();
        CenterMiscItems();

        SetStartTimeLeft();

        SetCommands();

        GridManager.Instance.SetCellsVisibility();
    }

    #region Initialize Methods
    private void SpawnTutorialBox()
    {
        GameObject go = new()
        {
            Type = GameObjectTypes.Gui,
        };
        go.AddComponent<SpriteRenderer>();
        go.AddComponent<Collider>();
        go.AddComponent<TutorialBox>();
        GameWorld.Instance.Instantiate(go);
    }
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
        RoomSpawner = spawnerGo.AddComponent<Spawner>();
        EnemiesInRoom = RoomSpawner.SpawnEnemies(EnemySpawnPoints, PlayerGo, _spawnAbleTypes, EnemyWeakness);
    }
    public float EnemyWeakness { get; protected set; } = 0.3f;
    private static List<EnemyTypes> _spawnAbleTypes = new()
    {
        EnemyTypes.OrcArcher,
        EnemyTypes.OrcWarrior,
    };

    private void SpawnPotions()
    {
        RoomSpawner.SpawnPotions(PotionSpawnPoints, PlayerGo, _spawnablePotionTypes);
    }

    private static List<PotionTypes> _spawnablePotionTypes = new()
    {
        PotionTypes.BigHealth,
        PotionTypes.SmallHealth,
    };

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
        Player = PlayerGo.GetComponent<Player>();
        _playerHealth = PlayerGo.GetComponent<Health>();
        InputHandler.Instance.AddKeyUpdateCommand(RightMovementKey, new MoveCmd(Player, new Vector2(1, 0)));
        InputHandler.Instance.AddKeyUpdateCommand(LeftMovementKey, new MoveCmd(Player, new Vector2(-1, 0)));
        InputHandler.Instance.AddKeyUpdateCommand(UpMovementKey, new MoveCmd(Player, new Vector2(0, -1)));
        InputHandler.Instance.AddKeyUpdateCommand(DownMovementKey, new MoveCmd(Player, new Vector2(0, 1)));

        InputHandler.Instance.AddMouseUpdateCommand(AttackSimpelAttackKey, new CustomCmd(Player.Attack));

        InputHandler.Instance.AddKeyButtonDownCommand(TogglePauseMenuKey, new CustomCmd(_pauseMenu.TogglePauseMenu));
        InputHandler.Instance.AddKeyUpdateCommand(ToggleStatsMenuKey, new CustomCmd(ToggleStatsMenu));

        InputHandler.Instance.AddKeyButtonDownCommand(UseItem, new CustomCmd(Player.UseItem));

        InputHandler.Instance.AddKeyUpdateCommand(DashKey, new CustomCmd(Player.UpdateDash));

        // For debugging
        if (!GameWorld.DebugAndCheats) return;

        //InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space, new CustomCmd(_player.Attack));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Enter, new CustomCmd(ChangeScene));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.O, new CustomCmd(() => { DB.Instance.SaveGrid(GridManager.Instance.CurrentGrid); }));

        //InputHandler.Instance.AddKeyButtonDownCommand(Keys.Q, new CustomCmd(() => { player.GameObject.GetComponent<Health>().TakeDamage(rnd.Next(500000000, 500000000)); }));
    }
    private void ToggleStatsMenu()
    {
        _showStats = true;
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
            _playerHealth.TakeDamage(1000, Player.GameObject.Transform.Position + new Vector2(30, -30)); // Kills the player
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

        DrawPlayerStats(spriteBatch);   

        if (!InputHandler.Instance.DebugMode) return;
        DrawDebug(spriteBatch);
    }

    /// <summary>
    /// Needs to set the QuestText variable
    /// </summary>
    protected virtual void SetQuestLogText()
    {
        _aliveEnemies = EnemiesInRoom.Where(x => x.State != CharacterState.Dead).ToList();

        int dead = EnemiesInRoom.Count - _aliveEnemies.Count;
        int amountToKill = EnemiesInRoom.Count - dead;

        QuestText = $"Enemies left {amountToKill}/{EnemiesInRoom.Count}";//
    }

    private void DrawQuest(SpriteBatch spriteBatch)
    {
        SetQuestLogText();
        Vector2 size = GlobalTextures.DefaultFont.MeasureString(QuestText);
        Vector2 textPos = GameWorld.Instance.UiCam.TopRight + new Vector2(-260, 55);

        SpriteRenderer.DrawCenteredSprite(spriteBatch, TextureNames.QuestUnder, textPos, BaseMath.TransitionColor(Color.White), LayerDepth.Default);
        GuiMethods.DrawTextCentered(spriteBatch, GlobalTextures.DefaultFont, textPos, QuestText, CurrentTextColor);
    }

    private void DrawTimer(SpriteBatch spriteBatch, Vector2 timerPos)
    {
        TimeSpan time = TimeSpan.FromSeconds(SaveData.Time_Left);
        string finalText = $"Time Left: {time.Minutes:D2}:{time.Seconds:D2}";

        Vector2 lineSize = GlobalTextures.DefaultFont.MeasureString(finalText);
        Vector2 center = timerPos - new Vector2(0, lineSize.Y / 2);

        spriteBatch.DrawString(GlobalTextures.DefaultFont, finalText, center, CurrentTextColor);
    }

    private void DrawPlayerStats(SpriteBatch spriteBatch)
    {
        if (!_showStats || Player == null) return;

        Vector2 startPos = GameWorld.Instance.UiCam.TopLeft + new Vector2(40, 200);
        string introText = $"Name: {Player.Name}\n\nClass: {Player.ClassType}";
        string bodyText = $"Speed: {Player.SpeedMultiplier}x\n\nDamage: {Player.DamageMultiplier}x";
        float layer = SpriteRenderer.GetLayerDepth(LayerDepth.Text);

        spriteBatch.Draw(Player.StatsScreen, startPos, null, BaseMath.TransitionColor(Color.White), 0f, Vector2.Zero, 4, SpriteEffects.None, SpriteRenderer.GetLayerDepth(LayerDepth.UI));
        
        spriteBatch.DrawString(GlobalTextures.DefaultFont, introText, startPos + new Vector2(110, 30), CurrentTextColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, layer);

        spriteBatch.DrawString(GlobalTextures.DefaultFont, bodyText, startPos + new Vector2(20, 120), CurrentTextColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, layer);

        _showStats = false;
    }

    private void DrawDebug(SpriteBatch spriteBatch)
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
        DrawString(spriteBatch, $"Player GirdPos: {PlayerGo.Transform.GridPosition}", startPos);

        //foreach (Vector2 retanglePos in PlayerGo.GetComponent<Collider>().CollisionBox

        if (Player.movementCollider != null)
        {
            Rectangle collisionBox = Player.movementCollider.CollisionBox;
            // Check each corner of the CollisionBox
            Vector2[] corners = new Vector2[]
            {
            new(collisionBox.Left, collisionBox.Top),
            new(collisionBox.Right, collisionBox.Top),
            new(collisionBox.Right, collisionBox.Bottom),
            new(collisionBox.Left, collisionBox.Bottom)
            };

            for (int i = 0; i < corners.Length; i++)
            {
                Cell cellUnderCorner = GridManager.Instance.CurrentGrid.GetCellFromPos(corners[i]);

                bool canWalk = false;
                if (cellUnderCorner != null && cellUnderCorner.CellWalkableType == CellWalkableType.FullValid)
                {
                    canWalk = true;
                }

                startPos += offset;
                DrawString(spriteBatch, $"PlayerMovement CORNER{i + 1} CAN WALK: {canWalk}", startPos);
            }
        }


        startPos += offset;
        DrawString(spriteBatch, $"Player Room Nr {Player.CollisionNr}", startPos);

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
        DrawString(spriteBatch, $"player.velocity {Player.velocity}", startPos);

    }

    protected void DrawString(SpriteBatch spriteBatch, string text, Vector2 position)
    {
        spriteBatch.DrawString(GlobalTextures.DefaultFont, text, position, Color.Pink, 0f, Vector2.Zero, 1, SpriteEffects.None, SpriteRenderer.GetLayerDepth(LayerDepth.Text));
    }

    #endregion Draw
}