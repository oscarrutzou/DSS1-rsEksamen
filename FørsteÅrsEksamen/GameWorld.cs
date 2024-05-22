using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.GameManagement.Scenes;
using FørsteÅrsEksamen.GameManagement.Scenes.Menus;
using FørsteÅrsEksamen.GameManagement.Scenes.Rooms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FørsteÅrsEksamen.GameManagement
{
    // Oscar
    public class GameWorld : Game
    {
        public static GameWorld Instance;

        public Dictionary<ScenesNames, Scene> Scenes { get; private set; }
        public Scene[] Rooms { get; private set; }

        public Scene CurrentScene;
        public Camera WorldCam { get; private set; }
        public Camera UiCam { get; private set; } //Static on the ui
        public static float DeltaTime { get; private set; }
        public GraphicsDeviceManager GfxManager { get; private set; }
        private SpriteBatch _spriteBatch;
        private ScenesNames? nextScene = null;
        private int nextRoom;
        private bool changedSceneRoom;
        public GameWorld()
        {
            GfxManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "A-Content";
            IsMouseVisible = true;
            Window.Title = "Doctor's Dungeon";
        }

        protected override void Initialize()
        {
            SceneData.GenereateGameObjectDicionary();

            ResolutionSize(1280, 720);
            //Fullscreen();
            WorldCam = new Camera(true);
            UiCam = new Camera(false);

            GlobalTextures.LoadContent();
            GlobalSounds.LoadContent();
            GlobalAnimations.LoadContent();

            GenerateScenes();

            CurrentScene = Scenes[ScenesNames.MainMenu];
            CurrentScene.Initialize();

            // Start Input Handler Thread
            Thread cmdThread = new(() => { InputHandler.Instance.StartInputThead(); })
            {
                IsBackground = true // Stops the thread abruptly
            };
            cmdThread.Start();

            base.Initialize();
        }

        // Simpel lås verision for at ikke åbne 2 versioner af spillet, brug mutex

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            GlobalSounds.MusicUpdate(); // Maybe run on own thread?

            CurrentScene.Update(gameTime);

            HandleSceneChange();
            HandleRoomChange();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            CurrentScene.DrawSceenColor();

            //Draw in world objects. Use pixel perfect and a WorldCam, that can be moved around
            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
                transformMatrix: WorldCam.GetMatrix());

            CurrentScene.DrawInWorld(_spriteBatch);
            _spriteBatch.End();

            //Draw on screen objects. Use pixel perfect and a stationary UiCam that dosent move around
            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
                transformMatrix: UiCam.GetMatrix());

            CurrentScene.DrawOnScreen(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Generates the scenes that can be used in the project.
        /// </summary>
        private void GenerateScenes()
        {
            Rooms = new Scene[3];
            Rooms[0] = new Room1Scene();

            Scenes = new Dictionary<ScenesNames, Scene>();
            Scenes[ScenesNames.MainMenu] = new MainMenu();

            Scenes[ScenesNames.WeaponTestScene] = new WeaponTestScene();
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
        public void Instantiate(GameObject go) => CurrentScene.Instantiate(go);

        /// <summary>
        /// Removes the GameObject from the CurrrentScene
        /// </summary>
        /// <param name="go"></param>
        public void Destroy(GameObject go) => CurrentScene.Destroy(go);

        public void ChangeScene(ScenesNames sceneName) => nextScene = sceneName;


        /// <summary>
        /// A method to prevent changing in the GameObject lists while its still inside the Update
        /// </summary>
        private void HandleSceneChange()
        {
            if (nextScene == null) return;

            SceneData.DeleteAllGameObjects();
            CurrentScene = Scenes[nextScene.Value];
            CurrentScene.Initialize();
            nextScene = null;
        }

        public void ChangeRoom(int roomReached)
        {
            nextRoom = roomReached;
            changedSceneRoom = true;
        }

        private void HandleRoomChange()
        {
            if (!changedSceneRoom) return;

            changedSceneRoom = false;
            GlobalSounds.InMenu = false;
            SceneData.DeleteAllGameObjects();
            CurrentScene = Rooms[nextRoom];
            CurrentScene.Initialize();
        }
    }
}