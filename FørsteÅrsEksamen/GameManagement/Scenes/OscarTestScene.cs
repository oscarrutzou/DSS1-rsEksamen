using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.ComponentPattern.Grid;
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
        private GameObject playerGo;

        private Vector2 playerPos;

        public override void Initialize()
        {
            MakePlayer();
            GridManager.Instance.InitalizeGrids();
            SetCommands();
        }

        private void MakePlayer()
        {
            playerFactory = new PlayerFactory();
            playerGo = playerFactory.Create();
            GameWorld.Instance.Instantiate(playerGo);
        }

        private Player player;

        private void SetCommands()
        {
            player = playerGo.GetComponent<Player>();
            player.Attach(this);
            InputHandler.Instance.AddKeyUpdateCommand(Keys.D, new MoveCommand(player, new Vector2(1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.A, new MoveCommand(player, new Vector2(-1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.W, new MoveCommand(player, new Vector2(0, -1)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.S, new MoveCommand(player, new Vector2(0, 1)));
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

        private List<GameObject> list;

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"PlayerPos {playerPos}", GameWorld.Instance.UiCam.TopLeft, Color.Black);

            SceneData.GameObjectLists.TryGetValue(GameObjectTypes.Cell, out list);
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"SceneObjects in scene {list.Count}", GameWorld.Instance.UiCam.TopLeft + new Vector2(0, 60), Color.Black);

            base.DrawOnScreen(spriteBatch);
        }

        public void UpdateObserver()
        {
            playerPos = player.GameObject.Transform.Position;
        }
    }
}