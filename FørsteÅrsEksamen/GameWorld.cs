using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.GameManagement.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.GameManagement
{
    // Oscar
    public class GameWorld : Game
    {
        public static GameWorld Instance;

        public Dictionary<ScenesNames, Scene> Scenes { get; private set; }
        public Scene currentScene;
        public Camera WorldCam { get; set; }
        public Camera UiCam { get; private set; } //Static on the ui
        public static float DeltaTime { get; private set; }
        public GraphicsDeviceManager GfxManager { get; private set; }
        private SpriteBatch _spriteBatch;

        public GameWorld()
        {
            GfxManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "A-Content";
            IsMouseVisible = true;
            Window.Title = "Første års eksamen";
        }

        protected override void Initialize()
        {
            SceneData.GenereateGameObjectDicionary();

            ResolutionSize(1280, 720);
            //Fullscreen();
            WorldCam = new Camera(true);
            UiCam = new Camera(false);

            GlobalTextures.LoadContent();
            GlobalAnimations.LoadContent();

            GenerateScenes();
            ChangeScene(ScenesNames.OscarTestScene);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            InputHandler.Instance.Update(); //Updates our input, so its not each scene that have to handle the call.
            currentScene.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            currentScene.DrawSceenColor();

            //Draw in world objects. Use pixel perfect and a WorldCam, that can be moved around
            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
                transformMatrix: WorldCam.GetMatrix());

            currentScene.DrawInWorld(_spriteBatch);
            _spriteBatch.End();

            //Draw on screen objects. Use pixel perfect and a stationary UiCam that dosent move around
            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
                transformMatrix: UiCam.GetMatrix());

            currentScene.DrawOnScreen(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Generates the scenes that can be used in the project.
        /// </summary>
        private void GenerateScenes()
        {
            Scenes = new Dictionary<ScenesNames, Scene>();
            Scenes[ScenesNames.GameScene] = new GameScene();
            Scenes[ScenesNames.OscarTestScene] = new OscarTestScene();
            Scenes[ScenesNames.ErikTestScene] = new ErikTestScene();
            Scenes[ScenesNames.StefanTestScene] = new StefanTestScene();
            Scenes[ScenesNames.AsserTestScene] = new AsserTestScene();
        }

        public void ResolutionSize(int width, int height)
        {
            GfxManager.HardwareModeSwitch = true;
            GfxManager.PreferredBackBufferWidth = width;
            GfxManager.PreferredBackBufferHeight = height;
            GfxManager.IsFullScreen = false;
            GfxManager.ApplyChanges();
        }

        /// <summary>
        /// Sets the project to fullscreen on Load
        /// </summary>
        public void Fullscreen()
        {
            GfxManager.HardwareModeSwitch = false;
            GfxManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            GfxManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            GfxManager.IsFullScreen = true;
            GfxManager.ApplyChanges();
        }

        /// <summary>
        /// Adds the GameObject to the CurrentScene
        /// </summary>
        /// <param name="go"></param>
        public void Instantiate(GameObject go) => currentScene.Instantiate(go);

        /// <summary>
        /// Removes the GameObject from the CurrrentScene
        /// </summary>
        /// <param name="go"></param>
        public void Destroy(GameObject go) => currentScene.Destroy(go);

        /// <summary>
        /// Deletes the current GameObjects and starts the new Scene
        /// </summary>
        /// <param name="sceneName"></param>
        public void ChangeScene(ScenesNames sceneName)
        {
            SceneData.DeleteAllGameObjects();
            currentScene = Scenes[sceneName];
            currentScene.Initialize();
        }
    }
}