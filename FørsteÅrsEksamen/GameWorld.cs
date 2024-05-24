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
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.GameManagement
{
    // Oscar
    public class GameWorld : Game
    {
        public static GameWorld Instance;

        public Dictionary<SceneNames, Scene> Scenes { get; private set; }

        public Scene CurrentScene;
        public Camera WorldCam { get; private set; }
        public Camera UiCam { get; private set; } //Static on the ui
        public static float DeltaTime { get; private set; }
        public GraphicsDeviceManager GfxManager { get; private set; }
        public SemaphoreSlim InputHandlerSemaphore = new SemaphoreSlim(1, 1);

        private SpriteBatch _spriteBatch;
        private SceneNames? nextScene = null;

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

            //ResolutionSize(1280, 720);
            Fullscreen();
            WorldCam = new Camera(true);
            UiCam = new Camera(false);

            GlobalTextures.LoadContent();
            GlobalSounds.LoadContent();
            GlobalAnimations.LoadContent();

            GenerateScenes();

            CurrentScene = Scenes[SceneNames.MainMenu];
            CurrentScene.Initialize();

            Thread inputThread = new Thread(InputHandler.Instance.StartInputThread)
            {
                IsBackground = true // Stops the thread abruptly
            };
            inputThread.Start();

            base.Initialize();
        }

        // Simpel lås verision for at ikke åbne 2 versioner af spillet, brug mutex

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override async void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            GlobalSounds.MusicUpdate(); // Maybe run on own thread?

            CurrentScene.Update(gameTime);

            await HandleSceneChange();

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

        #region Scene
        /// <summary>
        /// Generates the scenes that can be used in the project.
        /// </summary>
        private void GenerateScenes()
        {
            Scenes = new()
            {
                [SceneNames.MainMenu] = new MainMenu(),
                [SceneNames.SaveFileMenu] = new SaveFileMenu(),
                [SceneNames.CharacterSelectorMenu] = new CharacterSelectorMenu(),
                //Scenes[ScenesNames.LoadingScreen] = new ();
                //Scenes[ScenesNames.EndMenu] = new();
                [SceneNames.DungounRoom1] = new Room1Scene(),
                [SceneNames.DungounRoom2] = new Room2Scene(),

                // Test scenes
                [SceneNames.WeaponTestScene] = new WeaponTestScene(),
                [SceneNames.OscarTestScene] = new OscarTestScene(),
                [SceneNames.ErikTestScene] = new ErikTestScene(),
                [SceneNames.StefanTestScene] = new StefanTestScene(),
                [SceneNames.AsserTestScene] = new AsserTestScene()
            };
        }

        public void ChangeScene(SceneNames sceneName)
        {
            // Is last char of enum a digit (^1 is the same as sceneString.Length - 1)
            if (char.IsDigit(sceneName.ToString()[^1]))
                throw new Exception("Dont try and use this method to change between Dungoun Rooms. " +
                    "Summon the Wizard Oscar:)");

            nextScene = sceneName;
        }

        public void ChangeDungounScene(SceneNames baseRoomType, int roomReached)
        {
            string sceneNameString = baseRoomType.ToString();

            // Check if the scene name ends with a number
            if (char.IsDigit(sceneNameString[^1]))
            {
                // Extract the base name
                sceneNameString = string.Concat(sceneNameString.TakeWhile(c => !char.IsDigit(c)));
            }

            // Append the room number to the base name
            string newSceneName = sceneNameString + roomReached;

            // Try to parse the new scene name as a SceneNames enum value
            if (Enum.TryParse(newSceneName, out SceneNames newScene))
            {
                nextScene = newScene;

                if (!Scenes.ContainsKey(newScene)) // The next scene dosent exit
                {
                    nextScene = SceneNames.MainMenu; // Sends them back to the menu
                }
            }
            else
            {
                nextScene = SceneNames.MainMenu; // Sends them back to the menu
            }
        }

        /// <summary>
        /// A method to prevent changing in the GameObject lists while its still inside the Update
        /// </summary>
        private async Task HandleSceneChange()
        {
            if (nextScene == null || Scenes[nextScene.Value] == null) return;

            await InputHandlerSemaphore.WaitAsync();

            try
            {
                // Change the scene
                CurrentScene.OnSceneChange(); // Removes stuff like commands
                SceneData.DeleteAllGameObjects(); // Removes every object

                CurrentScene = Scenes[nextScene.Value]; // Changes to new scene
                CurrentScene.Initialize();
                nextScene = null;
            }
            finally
            {
                InputHandlerSemaphore.Release();
            }
        }

        #endregion
    }
}