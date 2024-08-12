using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.CommandPattern.Commands;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Enemies;
using DoctorsDungeon.ComponentPattern.Particles.BirthModifiers;
using DoctorsDungeon.ComponentPattern.Particles.Modifiers;
using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ComponentPattern.Particles;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.Factory;
using DoctorsDungeon.GameManagement.Scenes.Menus;
using DoctorsDungeon.LiteDB;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoctorsDungeon.GameManagement.Scenes.Rooms;

// Oscar
public abstract class RoomBase : Scene
{
    #region Properties

    private PauseMenu pauseMenu;

    protected string GridName;
    protected int GridWidth, GridHeight;
    protected TextureNames BackGroundTexture = TextureNames.TestLevelBG;
    protected TextureNames ForeGroundTexture = TextureNames.TestLevelFG;

    public Point PlayerSpawnPos, EndPointSpawnPos = new(6, 6);
    protected GameObject PlayerGo;
    private Player player;
    private Health playerHealth;

    protected List<Point> EnemySpawnPoints = new();
    protected List<Point> PotionSpawnPoints = new();
    protected Dictionary<Point, GameObject> MiscGameObjectsInRoom = new();

    private TransferDoor transferDoor {  get; set; }
    private SpriteRenderer transferDoorSpriteRenderer;
    private List<Enemy> enemiesInRoom = new();
    private List<Enemy> aliveEnemies;

    private Spawner spawner;

    private List<GameObject> cells = new(); // For debug

    #endregion Properties

    public override void Initialize()
    {
        //GameWorld.Instance.IsMouseVisible = false;

        SetSpawnPotions();

        // There needs to have been set some stuff before this base.Initialize (Look at Room1 for reference)
        PlayerGo = null; //Remove this from normal Scene and make another scene that sets all up.

        pauseMenu = new PauseMenu();
        pauseMenu.Initialize();
        OnFirstCleanUp = pauseMenu.AfterFirstCleanUp; // We need to couple the pausemenu to the current RoomScene Action.

        SpawnTexture(BackGroundTexture, LayerDepth.WorldBackground);
        SpawnTexture(ForeGroundTexture, LayerDepth.WorldForeground);

        SpawnGrid();

        SpawnAndLoadPlayer();

        SpawnEndPos();

        SpawnEnemies();
        SpawnPotions();
        CenterMiscItems();

        SetCommands();

        GridManager.Instance.SetCellsVisibility();
    }

    #region Initialize Methods

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

        if (GameWorld.Instance.BackgroundEmitter == null) return;
        GameWorld.Instance.BackgroundEmitter.FollowGameObject(PlayerGo, Vector2.Zero);
    }

    private void SpawnEndPos()
    {
        GameObject endDoor = TransferDoorFactory.Create();
        transferDoor = endDoor.GetComponent<TransferDoor>();
        transferDoorSpriteRenderer = endDoor.GetComponent<SpriteRenderer>();
        endDoor.Transform.Position = GridManager.Instance.GetCornerPositionOfCell(EndPointSpawnPos);
        GameWorld.Instance.Instantiate(endDoor);
    }

    private void SpawnEnemies()
    {
        GameObject spawnerGo = new();
        spawner = spawnerGo.AddComponent<Spawner>();
        enemiesInRoom = spawner.SpawnEnemies(EnemySpawnPoints, PlayerGo);
    }

    private void SpawnPotions()
    {
        spawner.SpawnPotions(PotionSpawnPoints, PlayerGo);
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
        player = PlayerGo.GetComponent<Player>();
        playerHealth = PlayerGo.GetComponent<Health>();
        InputHandler.Instance.AddKeyUpdateCommand(Keys.D, new MoveCmd(player, new Vector2(1, 0)));
        InputHandler.Instance.AddKeyUpdateCommand(Keys.A, new MoveCmd(player, new Vector2(-1, 0)));
        InputHandler.Instance.AddKeyUpdateCommand(Keys.W, new MoveCmd(player, new Vector2(0, -1)));
        InputHandler.Instance.AddKeyUpdateCommand(Keys.S, new MoveCmd(player, new Vector2(0, 1)));

        InputHandler.Instance.AddMouseUpdateCommand(MouseCmdState.Left, new CustomCmd(player.Attack));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(pauseMenu.TogglePauseMenu));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.E, new CustomCmd(player.UseItem));

        // For debugging
        if (!GameWorld.DebugAndCheats) return;

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space, new CustomCmd(player.Attack));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Enter, new CustomCmd(ChangeScene));
        InputHandler.Instance.AddKeyButtonDownCommand(Keys.O, new CustomCmd(() => { DB.Instance.SaveGrid(GridManager.Instance.CurrentGrid); }));

        InputHandler.Instance.AddKeyButtonDownCommand(Keys.Q, new CustomCmd(() => { player.GameObject.GetComponent<Health>().TakeDamage(rnd.Next(500000000, 500000000)); }));
    }
    Random rnd = new();

    private void ChangeScene()
    {
        int newRoomNr = SaveData.Level_Reached + 1;
        GameWorld.Instance.ChangeDungeonScene(SceneNames.DungeonRoom, newRoomNr);
    }

    #endregion Initialize Methods

    public override void Update(GameTime gameTime)
    {
        SaveData.Time_Left -= GameWorld.DeltaTime;

        if (SaveData.Time_Left <= 0) // Player ran out of Time
        {
            SaveData.Time_Left = 0;
            SaveData.LostByTime = true;
            playerHealth.TakeDamage(1000); // Kills the player
        }

        // Check if enemies has been killed
        aliveEnemies = enemiesInRoom.Where(x => x.State != CharacterState.Dead).ToList();

        if (aliveEnemies.Count == 0) // All enemies are dead to
        {
            OnAllEnemiesDied();
        }

        base.Update(gameTime);
    }

    private void OnAllEnemiesDied()
    {
        if (transferDoorSpriteRenderer.ShouldDrawSprite == false) return; // To stop method from being run twice.

        transferDoorSpriteRenderer.ShouldDrawSprite = false;
        transferDoor.CanTranser = true;
        transferDoor.emitter.StartEmitter();
    }

    #region Draw

    public override void DrawOnScreen(SpriteBatch spriteBatch)
    {
        base.DrawOnScreen(spriteBatch);

        pauseMenu.DrawOnScreen(spriteBatch);

        Vector2 leftPos = GameWorld.Instance.UiCam.TopLeft + new Vector2(30, 30);
        DrawTimer(spriteBatch, leftPos);

        leftPos += new Vector2(0, 30);
        spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Player HP: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}", leftPos, CurrentTextColor);

        leftPos += new Vector2(0, 30);
        DrawPotion(spriteBatch, leftPos);

        DrawQuest(spriteBatch);

        if (!InputHandler.Instance.DebugMode) return;
        DebugDraw(spriteBatch);
    }

    private void DrawQuest(SpriteBatch spriteBatch)
    {
        aliveEnemies = enemiesInRoom.Where(x => x.State != CharacterState.Dead).ToList();
        int amountToKill = EnemySpawnPoints.Count - aliveEnemies.Count;

        string text = $"Kill your way through {amountToKill}/{EnemySpawnPoints.Count}";
        Vector2 size = GlobalTextures.DefaultFont.MeasureString(text);
        Vector2 textPos = GameWorld.Instance.UiCam.TopRight + new Vector2(-size.X - 30, size.Y + 10);
        Vector2 underPos = textPos - new Vector2(45, 35);

        Color questUnderColor = Color.White;
        if (IsChangingScene)
            questUnderColor = Color.Lerp(Color.White, Color.Transparent, (float)TransitionProgress);

        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.QuestUnder], underPos, null, questUnderColor, 0f, Vector2.Zero, 6f, SpriteEffects.None, 0f);

        spriteBatch.DrawString(GlobalTextures.DefaultFont, text, textPos, CurrentTextColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
    }

    private void DrawTimer(SpriteBatch spriteBatch, Vector2 timerPos)
    {
        TimeSpan time = TimeSpan.FromSeconds(SaveData.Time_Left);
        string minutes = time.Minutes.ToString("D2");
        string seconds = time.Seconds.ToString("D2");
        spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Time Left: {minutes}:{seconds}", timerPos, CurrentTextColor);
    }

    private void DrawPotion(SpriteBatch spriteBatch, Vector2 intentoryPos)
    {
        string text;
        if (player.ItemInInventory == null)
        {
            text = "Inventory (0/1):";
        }
        else
        {
            text = $"Inventory (1/1): {player.ItemInInventory.Name}";
        }

        spriteBatch.DrawString(GlobalTextures.DefaultFont, text, intentoryPos, CurrentTextColor);
    }

    private void DebugDraw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], GameWorld.Instance.UiCam.TopLeft, null, Color.WhiteSmoke, 0f, Vector2.Zero, new Vector2(450, 350), SpriteEffects.None, 0.99f); // Over everything exept text

        Vector2 mousePos = InputHandler.Instance.MouseOnUI;

        Vector2 startPos = GameWorld.Instance.UiCam.TopLeft;
        Vector2 offset = new Vector2(0, 30);

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
        DrawString(spriteBatch, $"Player Room Nr {player.CollisionNr}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Cell GameObjects in scene {cells.Count}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Current Level Reached {SaveData.Level_Reached}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Grid Current Draw {GridManager.Instance.CurrentDrawSelected.ToString()}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Grid Collision Nr {GridManager.Instance.ColliderNrIndex}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Grid Room Nr {GridManager.Instance.RoomNrIndex}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Player TargetVel {player.targetVelocity}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"player.velocity {player.velocity}", startPos);

        startPos += offset;
        DrawString(spriteBatch, $"Player totalMovementInput: {player.totalMovementInput.X},{player.totalMovementInput.Y}", startPos);
    }

    protected void DrawString(SpriteBatch spriteBatch, string text, Vector2 position)
    {
        spriteBatch.DrawString(GlobalTextures.DefaultFont, text, position, Color.Pink, 0f, Vector2.Zero, 1, SpriteEffects.None, 1f);
    }

    #endregion Draw
}