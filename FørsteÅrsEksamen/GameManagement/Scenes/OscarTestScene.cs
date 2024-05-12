using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.ComponentPattern.Enemies.Skeleton;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.Factory.Gui;
using FørsteÅrsEksamen.ObserverPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.GameManagement.Scenes
{
    public class OscarTestScene : Scene, IObserver
    {
        private PlayerFactory playerFactory;
        private ButtonFactory buttonFactory;
        private GameObject playerGo, drawRoomBtn, drawAstarPathBtn;

        private Vector2 playerPos;

        public override void Initialize()
        {
            SetLevelBG();
            //First grid
            StartGrid();

            //Then player
            MakePlayer();

            // then enemies
            MakeEnemy();

            MakeButtons();
            SetCommands();
        }

        private void SetLevelBG()
        {
            GameObject go = new();
            go.Type = GameObjectTypes.Background;
            go.Transform.Scale = new(4, 4);

            SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.SetSprite(TextureNames.TestLevel);
            spriteRenderer.IsCentered = false;

            GameWorld.Instance.Instantiate(go);
        }

        private enum EnemyTypes
        {
            SkeletonWarrior,
        }

        private Dictionary<EnemyTypes, List<GameObject>> enemies = new();

        private void AddNewEnemy(EnemyTypes type, GameObject enemyGo)
        {
        }

        //private

        private void MakeEnemy()
        {
            EnemyFactory enemyFactory = new EnemyFactory();
            GameObject enemGo = enemyFactory.Create();
            GameWorld.Instance.Instantiate(enemGo);

            if (GridManager.Instance.CurrentGrid != null)
            {
                SkeletonWarrior enemy = enemGo.GetComponent<SkeletonWarrior>();
                enemy.SetStartPosition(playerGo, new Point(7, 13));
            }
        }

        private void MakePlayer()
        {
            Point spawn = new Point(6, 6);
            playerFactory = new PlayerFactory();
            playerGo = playerFactory.Create(PlayerClasses.Warrior);
            playerGo.Transform.Position = GridManager.Instance.CurrentGrid.Cells[spawn].Transform.Position;
            playerGo.Transform.GridPosition = spawn;
            GameWorld.Instance.WorldCam.position = playerGo.Transform.Position;
            GameWorld.Instance.Instantiate(playerGo);
        }

        private Player player;

        private void SetCommands()
        {
            player = playerGo.GetComponent<Warrior>();
            player.Attach(this);
            InputHandler.Instance.AddKeyUpdateCommand(Keys.D, new MoveCmd(player, new Vector2(1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.A, new MoveCmd(player, new Vector2(-1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.W, new MoveCmd(player, new Vector2(0, -1)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.S, new MoveCmd(player, new Vector2(0, 1)));
        }

        private void StartGrid()
        {
            GameObject gridGo = new();
            Grid grid = gridGo.AddComponent<Grid>("Test1", new Vector2(0, 0), 24, 18);
            grid.GenerateGrid();
            GridManager.Instance.SaveGrid(grid);
        }

        private void MakeButtons()
        {
            Camera uiCam = GameWorld.Instance.UiCam;

            buttonFactory = new();
            drawRoomBtn = buttonFactory.Create("Draw Room", () => { });
            drawRoomBtn.Transform.Translate(uiCam.TopRight + new Vector2(-100, 50));

            GameWorld.Instance.Instantiate(drawRoomBtn);

            drawAstarPathBtn = buttonFactory.Create("Draw Valid Path", () => { });
            drawAstarPathBtn.Transform.Translate(uiCam.TopRight + new Vector2(-100, 120));
            GameWorld.Instance.Instantiate(drawAstarPathBtn);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            GridManager.Instance.Update();
        }

        public override void DrawInWorld(SpriteBatch spriteBatch)
        {
            if (GridManager.Instance.CurrentGrid != null)
            {
                spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], GridManager.Instance.GetCornerPositionOfCell(new Point(3, 1)), null, Color.DarkRed, 0f, Vector2.Zero, 10, SpriteEffects.None, 1);

            }

            base.DrawInWorld(spriteBatch);
        }

        private List<GameObject> list; //For test
        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], GameWorld.Instance.UiCam.TopLeft, null, Color.WhiteSmoke, 0f, Vector2.Zero, new Vector2(350, 150), SpriteEffects.None, 0f);

            Vector2 mousePos = InputHandler.Instance.mouseInWorld;
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"MousePos {mousePos}", GameWorld.Instance.UiCam.TopLeft, Color.Black);

            DrawCellPos(spriteBatch);

            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"PlayerPos {playerPos}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 60), Color.Black);

            SceneData.GameObjectLists.TryGetValue(GameObjectTypes.Cell, out list);
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Cell GameObjects in scene {list.Count}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 90), Color.Black);

            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"RoomNr {GridManager.Instance.RoomNrIndex}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 120), Color.Black);

            base.DrawOnScreen(spriteBatch);
        }

        private void DrawCellPos(SpriteBatch spriteBatch)
        {
            //if (GridManager.Instance.CurrentGrid == null) throw new System.Exception("Error spørg da Oscar");
            GameObject cellGo = GridManager.Instance.GetCellAtPos(InputHandler.Instance.mouseInWorld);

            if (cellGo == null) return;

            Point cellGridPos = cellGo.Transform.GridPosition;
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Cell Point from MousePos: {cellGridPos}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 30), Color.Black);
        }

        public void UpdateObserver()
        {
            playerPos = player.GameObject.Transform.Position;
        }
    }
}