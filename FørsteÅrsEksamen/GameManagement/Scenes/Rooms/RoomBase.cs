using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.CommandPattern.Commands;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.LiteDB;
using DoctorsDungeon.Factory;
using DoctorsDungeon.GameManagement.Scenes.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using DoctorsDungeon.ComponentPattern.Enemies;
using DoctorsDungeon.Other;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using System.Linq;

namespace DoctorsDungeon.GameManagement.Scenes.Rooms
{
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

        protected List<Point> enemySpawnPoints = new();
        protected List<Point> potionSpawnPoints = new();

        private TransferDoor transferDoor;
        private SpriteRenderer transferDoorSpriteRenderer;
        private List<Enemy> enemiesInRoom = new();

        private Spawner spawner;

        private List<GameObject> cells = new(); // For debug
        #endregion

        public override void Initialize()
        {
            GameWorld.Instance.IsMouseVisible = false;

            SetSpawnPotions();

            // There needs to have been set some stuff before this base.Initialize (Look at Room1 for reference)
            PlayerGo = null; //Remove this from normal Scene and make another scene that sets all up.

            pauseMenu = new PauseMenu();
            pauseMenu.Initialize();
            OnFirstCleanUp = pauseMenu.AfterFirstCleanUp;

            SpawnTexture(BackGroundTexture, LayerDepth.WorldBackground);
            SpawnTexture(ForeGroundTexture, LayerDepth.WorldForeground);
            
            SpawnGrid();
            
            SpawnPlayer();
            
            SpawnEndPos();
            
            SpawnEnemies();
            SpawnPotions();

            SetCommands();

            DBMethods.RegularSave();
        }

        #region Initialize Methods
        protected abstract void SetSpawnPotions();
        private void SpawnTexture(TextureNames textureName, LayerDepth layerDepth)
        {
            GameObject backgroundGo = new()
            {
                Type = GameObjectTypes.Background
            };
            backgroundGo.Transform.Scale = new(4, 4);

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
            GridManager.Instance.SaveGrid(grid);
        }

        private void SpawnPlayer()
        {
            PlayerData playerData = DBRunData.LoadPlayerData(SaveData.CurrentSaveID);

            if (playerData != null)
            {
               PlayerGo = DBMethods.SpawnPlayer(playerData, PlayerSpawnPos);
            }
            else
            {
                // We already know a exploit here.
                // If the user deletes the playerdata in the middle of a run, they will get a new Player with full stats
                // and therefore not load the old saved one. We estimate it will take too long to fix this, so we place it on the back burner for now.
                PlayerGo = PlayerFactory.Create(SaveData.SelectedClass, SaveData.SelectedWeapon);
            }

            PlayerGo.Transform.Position = GridManager.Instance.CurrentGrid.Cells[PlayerSpawnPos].Transform.Position;
            PlayerGo.Transform.GridPosition = PlayerSpawnPos;
            GameWorld.Instance.WorldCam.Position = PlayerGo.Transform.Position;
            GameWorld.Instance.Instantiate(PlayerGo);
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
            enemiesInRoom = spawner.SpawnEnemies(enemySpawnPoints, PlayerGo);
        }

        private void SpawnPotions()
        {
            spawner.SpawnPotions(potionSpawnPoints, PlayerGo);
        }

        private void SetCommands()
        {
            player = PlayerGo.GetComponent<Player>();
            InputHandler.Instance.AddKeyUpdateCommand(Keys.D, new MoveCmd(player, new Vector2(1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.A, new MoveCmd(player, new Vector2(-1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.W, new MoveCmd(player, new Vector2(0, -1)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.S, new MoveCmd(player, new Vector2(0, 1)));

            InputHandler.Instance.AddMouseUpdateCommand(MouseCmdState.Left, new CustomCmd(player.Attack));

            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(pauseMenu.TogglePauseMenu));

            InputHandler.Instance.AddKeyButtonDownCommand(Keys.E, new CustomCmd(player.UseItem));

            // For debugging
            //InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space, new CustomCmd(player.Attack));
            //InputHandler.Instance.AddKeyButtonDownCommand(Keys.Enter, new CustomCmd(ChangeScene));
            //InputHandler.Instance.AddKeyButtonDownCommand(Keys.O, new CustomCmd(() => { DBGrid.SaveGrid(GridManager.Instance.CurrentGrid); }));
        }
        private void ChangeScene()
        {
            int newRoomNr = SaveData.Level_Reached + 1;
            GameWorld.Instance.ChangeDungeonScene(SceneNames.DungeonRoom, newRoomNr);
        }
        #endregion
        private List<Enemy> aliveEnemies;
        
        
        public override void Update(GameTime gameTime)
        {
            SaveData.Time_Left -= GameWorld.DeltaTime;

            if (SaveData.Time_Left <= 0) // Player ran out of Time
            {
                SaveData.Time_Left = 0;
                SaveData.LostByTime = true;
                player.TakeDamage(1000); // Kills the player
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
            if (transferDoorSpriteRenderer.ShouldDraw == false) return; // To stop method from being run twice.

            transferDoorSpriteRenderer.ShouldDraw = false;
            transferDoor.CanTranser = true;
        }

        #region Draw
        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);

            pauseMenu.DrawOnScreen(spriteBatch);

            Vector2 leftPos = GameWorld.Instance.UiCam.TopLeft + new Vector2(30, 30);
            DrawTimer(spriteBatch, leftPos);
            
            leftPos += new Vector2(0, 30);
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Player HP: {player.CurrentHealth}/{player.MaxHealth}", leftPos, Color.Red);
            
            leftPos += new Vector2(0, 30);
            DrawPotion(spriteBatch, leftPos);

            DrawQuest(spriteBatch);
            
            if (!InputHandler.Instance.DebugMode) return;
            DebugDraw(spriteBatch);
        }

        private void DrawQuest(SpriteBatch spriteBatch)
        {
            aliveEnemies = enemiesInRoom.Where(x => x.State != CharacterState.Dead).ToList();
            int amountToKill = enemySpawnPoints.Count - aliveEnemies.Count;

            string text = $"Kill your way though {amountToKill}/{enemySpawnPoints.Count}";
            Vector2 size = GlobalTextures.DefaultFont.MeasureString(text);
            Vector2 pos = GameWorld.Instance.UiCam.TopRight + new Vector2(-size.X - 30, size.Y + 10);
            spriteBatch.DrawString(GlobalTextures.DefaultFont, text, pos, Color.Red);
        }
        private void DrawTimer(SpriteBatch spriteBatch, Vector2 timerPos)
        {
            TimeSpan time = TimeSpan.FromSeconds(SaveData.Time_Left);
            string minutes = time.Minutes.ToString("D2");
            string seconds = time.Seconds.ToString("D2");
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Time Left: {minutes}:{seconds}", timerPos, Color.Red);
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

            spriteBatch.DrawString(GlobalTextures.DefaultFont, text, intentoryPos, Color.Red);
        }
        private void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], GameWorld.Instance.UiCam.TopLeft, null, Color.WhiteSmoke, 0f, Vector2.Zero, new Vector2(400, 300), SpriteEffects.None, 0.99f); // Over everything exept text

            Vector2 mousePos = InputHandler.Instance.MouseOnUI;

            DrawString(spriteBatch, $"MousePos UI {mousePos}", GameWorld.Instance.UiCam.TopLeft);

            GameObject cellGo = GridManager.Instance.GetCellAtPos(InputHandler.Instance.MouseInWorld);
            if (cellGo != null)
            {
                Point cellGridPos = cellGo.Transform.GridPosition;
                DrawString(spriteBatch, $"Cell Point from MousePos: {cellGridPos}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 30));
            }

            DrawString(spriteBatch, $"PlayerPos {PlayerGo.Transform.Position}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 60));
            DrawString(spriteBatch, $"Cell GameObjects in scene {cells.Count}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 90));
            DrawString(spriteBatch, $"LevelNr {GridManager.Instance.LevelNrIndex}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 120));
            DrawString(spriteBatch, $"Current Level Reached {SaveData.Level_Reached}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 150));
            DrawString(spriteBatch, $"Player Room Nr {player.RoomNr}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 180));
        }
        protected void DrawString(SpriteBatch spriteBatch, string text, Vector2 position)
        {
            spriteBatch.DrawString(GlobalTextures.DefaultFont, text, position, Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 1f);
        }
        #endregion
    }
}
