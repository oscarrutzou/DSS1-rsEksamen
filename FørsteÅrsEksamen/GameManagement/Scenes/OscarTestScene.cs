﻿using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.Factory;
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
        private GameObject level, playerGo, drawRoomBtn, drawAstarPathBtn;
        
        private Vector2 playerPos;

        public override void Initialize()
        {
            SetLevelBG();
            StartGrid();

            MakePlayer();
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

        private void MakePlayer()
        {
            playerFactory = new PlayerFactory();
            playerGo = playerFactory.Create();
            playerGo.Transform.Position = GridManager.Instance.CurrentGrid.Cells[new Point(3,3)].Transform.Position;
            GameWorld.Instance.WorldCam.position = playerGo.Transform.Position;
            GameWorld.Instance.Instantiate(playerGo);
        }

        private Player player;

        private void SetCommands()
        {
            player = playerGo.GetComponent<Player>();
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
        }

        public override void DrawInWorld(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], Vector2.Zero, Color.Black);

            base.DrawInWorld(spriteBatch);
        }

        private List<GameObject> list; //For test

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
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