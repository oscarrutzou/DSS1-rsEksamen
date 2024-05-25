using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.PlayerClasses;
using FørsteÅrsEksamen.ComponentPattern.Enemies.MeleeEnemies;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.ObserverPattern;
using FørsteÅrsEksamen.LiteDB;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using FørsteÅrsEksamen.Factory.Gui;
using FørsteÅrsEksamen.ComponentPattern.Enemies.RangedEnemies;

namespace FørsteÅrsEksamen.GameManagement.Scenes.TestScenes
{
    public class OscarTestScene : Scene, IObserver
    {
        private GameObject drawRoomBtn, drawAstarPathBtn;

        private Vector2 playerPos;
        private Point PlayerSpawnPos;
        private GameObject PlayerGo;

        public OscarTestScene()
        {
            PlayerSpawnPos = new Point(6, 6);

        }

        public override void Initialize()
        {
            SetLevelBG();
            //First grid
            StartGrid();

            //Then player
            MakePlayer();

            // then enemies
            MakeEnemy();

            MakeItem();

            MakeButtons();

            OnPlayerChanged();

            //DBMethods.SaveGame();
        }

        private void SetLevelBG()
        {
            GameObject go = new();
            go.Type = GameObjectTypes.Background;
            go.Transform.Scale = new(4, 4);

            SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.SetSprite(TextureNames.TestLevelBG);
            spriteRenderer.IsCentered = false;

            GameWorld.Instance.Instantiate(go);
        }

        private void MakeEnemy()
        {
            GameObject enemGo = EnemyFactory.Create(EnemyTypes.OrcWarrior);
            GameWorld.Instance.Instantiate(enemGo);

            if (GridManager.Instance.CurrentGrid != null)
            {
                SkeletonWarrior enemy = enemGo.GetComponent<SkeletonWarrior>();
                enemy.SetStartPosition(PlayerGo, new Point(7, 13));
            }
        }

        private void MakeItem()
        {
            GameObject itemGo = ItemFactory.Create(PlayerGo);
            GameWorld.Instance.Instantiate(itemGo);

            itemGo.Transform.Position = GridManager.Instance.CurrentGrid.Cells[new Point(3, 3)].Transform.Position;
        }

        private void MakePlayer()
        {
            PlayerGo = PlayerFactory.Create(ClassTypes.Warrior, WeaponTypes.Sword);
            PlayerGo.Transform.Position = GridManager.Instance.CurrentGrid.Cells[PlayerSpawnPos].Transform.Position;
            PlayerGo.Transform.GridPosition = PlayerSpawnPos;
            GameWorld.Instance.WorldCam.Position = PlayerGo.Transform.Position;
            GameWorld.Instance.Instantiate(PlayerGo);
        }

        private Player player;

        private void SetCommands()
        {
            player = PlayerGo.GetComponent<Player>();
            player.Attach(this);
            InputHandler.Instance.AddKeyUpdateCommand(Keys.D, new MoveCmd(player, new Vector2(1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.A, new MoveCmd(player, new Vector2(-1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.W, new MoveCmd(player, new Vector2(0, -1)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.S, new MoveCmd(player, new Vector2(0, 1)));

            InputHandler.Instance.AddKeyButtonDownCommand(Keys.D1, new CustomCmd(player.UseItem));
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Tab, new CustomCmd(() => { GridManager.Instance.ShowHideGrid(); }));
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.Space, new CustomCmd(Attack));
            InputHandler.Instance.AddKeyButtonDownCommand(Keys.O, new CustomCmd(() => { DBGrid.SaveGrid(GridManager.Instance.CurrentGrid); }));
        }

        public override void OnPlayerChanged()
        {
            InputHandler.Instance.RemoveAllExeptBaseCommands();
            SetCommands();
        }

        private void Attack()
        {
            //player.weapon.Attack();
        }

        private void TestRemoveComm()
        {
            InputHandler.Instance.RemoveKeyUpdateCommand(Keys.S);
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

            drawRoomBtn = ButtonFactory.Create("Draw Room", true, () => { });
            drawRoomBtn.Transform.Translate(uiCam.TopRight + new Vector2(-100, 50));

            GameWorld.Instance.Instantiate(drawRoomBtn);

            drawAstarPathBtn = ButtonFactory.Create("Draw Valid Path", true, () => { });
            drawAstarPathBtn.Transform.Translate(uiCam.TopRight + new Vector2(-100, 120));
            GameWorld.Instance.Instantiate(drawAstarPathBtn);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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

            Vector2 mousePos = InputHandler.Instance.MouseOnUI;
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"MousePos UI {mousePos}", GameWorld.Instance.UiCam.TopLeft, Color.Black);

            DrawCellPos(spriteBatch);

            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"PlayerPos {playerPos}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 60), Color.Black);

            SceneData.GameObjectLists.TryGetValue(GameObjectTypes.Cell, out list);
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Cell GameObjects in scene {list.Count}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 90), Color.Black);

            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"RoomNr {GridManager.Instance.LevelNrIndex}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 120), Color.Black);

            base.DrawOnScreen(spriteBatch);
        }

        private void DrawCellPos(SpriteBatch spriteBatch)
        {
            //if (GridManager.Instance.CurrentGrid == null) throw new System.Exception("Error spørg da Oscar");
            GameObject cellGo = GridManager.Instance.GetCellAtPos(InputHandler.Instance.MouseInWorld);

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