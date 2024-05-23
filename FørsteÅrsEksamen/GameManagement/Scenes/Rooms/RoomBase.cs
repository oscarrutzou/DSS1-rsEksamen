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
        protected GameObject PlayerGo;
        public Point PlayerSpawnPos;

        private PauseMenu pauseMenu;
        private Player player;

        protected TextureNames BackGroundTexture = TextureNames.TestLevel;
        protected string GridName = "Test1";
        protected int GridWidth = 24, GridHeight = 18;

        public override void Initialize()
        {
            PlayerGo = null; //Remove this from normal Scene and make another scene that sets all up.

            // Need to save the Rundata first

            // Then the player

            SpawnGridBG(); // Pull out all data that should be set and set it in initialize in a Dungoun Scene
            SpawnGrid();
            SpawnPlayer();
            SetCommands();

            DBMethods.RegularSave();

            pauseMenu = new PauseMenu();
            pauseMenu.Initialize();
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Escape, new CustomCmd(pauseMenu.TogglePauseMenu));
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Enter, new CustomCmd(ChangeScene));
        }

        private void ChangeScene()
        {
            int newRoomNr = Data.Room_Reached + 1;
            GameWorld.Instance.ChangeDungounScene(SceneNames.DungounRoom, newRoomNr);
        }

        private void SpawnGridBG()
        {
            GameObject go = new();
            go.Type = GameObjectTypes.Background;
            go.Transform.Scale = new(4, 4);

            SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.SetSprite(BackGroundTexture);
            spriteRenderer.IsCentered = false;

            GameWorld.Instance.Instantiate(go);
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

            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Tab, new CustomCmd(() => { GridManager.Instance.ShowHideGrid(); }));
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.O, new CustomCmd(() => { DBGrid.SaveGrid(GridManager.Instance.CurrentGrid); }));
        }


        private List<GameObject> cells = new();
        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            base.DrawOnScreen(spriteBatch);

            pauseMenu.DrawOnScreen(spriteBatch);

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
