using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.DB;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.GameManagement.Scenes.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Rooms
{
    public abstract class RoomBase : Scene
    {
        public Point PlayerSpawnPos;
        protected GameObject PlayerGo;
        private Player player;

        protected TextureNames BackGroundTexture = TextureNames.TestLevelBG;
        protected TextureNames ForeGroundTexture;
        protected string GridName;
        protected int GridWidth, GridHeight;
        
        private List<GameObject> cells = new(); // For debug
        private PauseMenu pauseMenu;

        public override void Initialize()
        {
            // There needs to have been set some stuff before this base.Initialize (Look at Room1 for reference)
            PlayerGo = null; //Remove this from normal Scene and make another scene that sets all up.

            pauseMenu = new PauseMenu();
            pauseMenu.Initialize();

            SpawnTexture(BackGroundTexture);
            //SpawnTexture(ForeGroundTexture);
            SpawnGrid();
            SpawnPlayer();
            SetCommands();

            DBMethods.RegularSave();
        }

        private void ChangeScene()
        {
            int newRoomNr = Data.Room_Reached + 1;
            GameWorld.Instance.ChangeDungounScene(SceneNames.DungounRoom, newRoomNr);
        }

        private void SpawnTexture(TextureNames textureName)
        {
            GameObject backgroundGo = new()
            {
                Type = GameObjectTypes.Background
            };
            backgroundGo.Transform.Scale = new(4, 4);

            SpriteRenderer spriteRenderer = backgroundGo.AddComponent<SpriteRenderer>();
            spriteRenderer.SetSprite(textureName);
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
            PlayerData playerData = DBRunData.LoadPlayerData(Data.CurrentSaveID);

            if (playerData != null)
            {
               PlayerGo = DBMethods.SpawnPlayer(playerData, PlayerSpawnPos);
            }
            else
            {
                // We already know a exploit here.
                // If the user deletes the playerdata in the middle of a run, they will get a new Player with full stats
                // and therefore not load the old saved one. We estimate it will take too long to fix this, so we place it on the back burner for now.
                PlayerGo = PlayerFactory.Create(Data.SelectedClass, Data.SelectedWeapon);
            }

            PlayerGo.Transform.Position = GridManager.Instance.CurrentGrid.Cells[PlayerSpawnPos].Transform.Position;
            PlayerGo.Transform.GridPosition = PlayerSpawnPos;
            GameWorld.Instance.WorldCam.position = PlayerGo.Transform.Position;
            GameWorld.Instance.Instantiate(PlayerGo);
        }

        private void SetCommands()
        {
            player = PlayerGo.GetComponent<Player>();
            InputHandler.Instance.AddKeyUpdateCommand(Keys.D, new MoveCmd(player, new Vector2(1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.A, new MoveCmd(player, new Vector2(-1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.W, new MoveCmd(player, new Vector2(0, -1)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.S, new MoveCmd(player, new Vector2(0, 1)));


            //InputHandler.Instance.AddMouseUpdateCommand(MouseCmdState.Left, new CustomCmd(player.Attack));
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space, new CustomCmd(player.Attack));

            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(pauseMenu.TogglePauseMenu));

            // For debugging
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Enter, new CustomCmd(ChangeScene));
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.I, new CustomCmd(() => { GridManager.Instance.ShowHideGrid(); }));
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.O, new CustomCmd(() => { DBGrid.SaveGrid(GridManager.Instance.CurrentGrid); }));
        }

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);

            pauseMenu.DrawOnScreen(spriteBatch);

            if (!InputHandler.Instance.DebugMode) return;

            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], GameWorld.Instance.UiCam.TopLeft, null, Color.WhiteSmoke, 0f, Vector2.Zero, new Vector2(350, 180), SpriteEffects.None, 0f);

            Vector2 mousePos = InputHandler.Instance.MouseOnUI;
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"MousePos UI {mousePos}", GameWorld.Instance.UiCam.TopLeft, Color.Black);

            DrawCellPos(spriteBatch);

            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"PlayerPos {PlayerGo.Transform.Position}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 60), Color.Black);

            
            SceneData.GameObjectLists.TryGetValue(GameObjectTypes.Cell, out cells);
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Cell GameObjects in scene {cells.Count}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 90), Color.Black);

            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"RoomNr {GridManager.Instance.RoomNrIndex}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 120), Color.Black);

            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Current Room Reached {Data.Room_Reached}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 150), Color.Black);
        }

        private void DrawCellPos(SpriteBatch spriteBatch)
        {
            //if (GridManager.Instance.CurrentGrid == null) throw new System.Exception("Error spørg da Oscar");
            GameObject cellGo = GridManager.Instance.GetCellAtPos(InputHandler.Instance.MouseInWorld);

            if (cellGo == null) return;

            Point cellGridPos = cellGo.Transform.GridPosition;
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Cell Point from MousePos: {cellGridPos}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 30), Color.Black);
        }
    }
}
