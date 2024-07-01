using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.GameManagement.Scenes;
using DoctorsDungeon.GameManagement.Scenes.Menus;
using DoctorsDungeon.GameManagement.Scenes.Rooms;
using DoctorsDungeon.GameManagement.Scenes.TestScenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace DoctorsDungeon;

// Alle
public class GameWorld : Game
{
    public static GameWorld Instance;
    public Dictionary<SceneNames, Scene> Scenes { get; private set; }

    public Scene CurrentScene;
    public Camera WorldCam { get; private set; } // Follows player 
    public Camera UiCam { get; private set; } //Static on the ui 
    public static float DeltaTime { get; private set; }
    public GraphicsDeviceManager GfxManager { get; private set; }

    public static bool IsPaused = false;
    public static readonly object GameobjectDeleteLock = new();

    public SceneNames? NextScene { get; private set; } = null;
    public bool ShowBG { get; set; } = true; // If we should show our background 

    private SpriteBatch _spriteBatch;
    private GameObject menuBackground;

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
        Fullscreen();

        WorldCam = new Camera(true); // Camera that follows the player
        UiCam = new Camera(false); // Camera that is static

        // Put some of this into threads to load faster in a loading menu, insted of running it here.
        GlobalTextures.LoadContent();
        GlobalSounds.LoadContent();
        GlobalAnimations.LoadContent();

        GenerateScenes(); // Makes a instance of all the scene we need

        SpawnBG(); // The background that dont get deleted

        CurrentScene = Scenes[SceneNames.MainMenu];
        CurrentScene.Initialize(); // Starts the main menu

        Thread inputThread = new(InputHandler.Instance.StartInputThread)
        {
            IsBackground = true // Stops the thread when main thread closes
        };
        inputThread.Start();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        GlobalSounds.MusicUpdate(); // Updates the Music in the game, not SFX

        CurrentScene.Update(gameTime); // Updates all gameobjects and their componetents in the scene

        HandleSceneChange(); // Goes to the next scene

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        CurrentScene.DrawSceenColor();

        //Draw in world objects. Uses pixel perfect and a WorldCam, that can be moved around
        _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
            SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
            transformMatrix: WorldCam.GetMatrix());

        CurrentScene.DrawInWorld(_spriteBatch);
        DrawBG(_spriteBatch);

        _spriteBatch.End();

        //Draw on screen objects. Use pixel perfect and a stationary UiCam that dosent move around
        _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
            SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
            transformMatrix: UiCam.GetMatrix());

        CurrentScene.DrawOnScreen(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public void SetResolutionSize(int width, int height)
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
        if (GraphicsDevice.DisplayMode.Width > 1920) // To big screen, so dont open in fullscreen
        {
            SetResolutionSize(1920, 1080);
            return;
        }

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
            [SceneNames.EndMenu] = new EndMenu(),
            //[SceneNames.LoadingScreen] = new LoadingScreen(),

            [SceneNames.DungeonRoom1] = new Room1Scene(),
            [SceneNames.DungeonRoom2] = new Room2Scene(),
            [SceneNames.DungeonRoom3] = new Room3Scene(),

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
        //if (char.IsDigit(sceneName.ToString()[^1]))
        //    throw new Exception("Dont try and use this method to change between Dungoun Rooms. " +
        //        "Summon the Wizard Oscar:)");

        NextScene = sceneName;
    }

    // Chosen to make it work with a base room type in the enum,
    // so we can easily change what kind of difficulty a room is and load in how far a player has come in a run.
    public void ChangeDungeonScene(SceneNames baseRoomType, int roomReached)
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
            NextScene = newScene;

            if (!Scenes.ContainsKey(newScene)) // The next scene dosent exit
            {
                NextScene = SceneNames.MainMenu; // Sends them back to the menu
            }
        }
        else
        {
            NextScene = SceneNames.MainMenu; // Sends them back to the menu
        }
    }

    /// <summary>
    /// A method to prevent changing in the GameObject lists while its still inside the Update
    /// </summary>
    private void HandleSceneChange()
    {
        if (NextScene == null || Scenes[NextScene.Value] == null) return;

        lock (GameobjectDeleteLock)
        {
            SceneData.DeleteAllGameObjects(); // Removes every object
        }
        // Remove all extra unnecessary Start code from lock to avoid a DeadLock.
        CurrentScene.OnSceneChange(); // Removes commands and more
        WorldCam.Position = Vector2.Zero; // Resets world cam position
        CurrentScene = Scenes[NextScene.Value]; // Changes to new scene
        CurrentScene.Initialize(); // Starts the new scene
        NextScene = null;
    }

    // We dont need a factory to do this, since its only this place we are going to use this background.
    private void SpawnBG()
    {
        menuBackground = new();
        menuBackground.Transform.Scale = new(4, 4);
        menuBackground.Type = GameObjectTypes.Background;
        SpriteRenderer sr = menuBackground.AddComponent<SpriteRenderer>();
        sr.SetLayerDepth(LayerDepth.WorldBackground);
        sr.SetSprite(TextureNames.SpaceBG1);

        menuBackground.Awake();
        menuBackground.Start();
    }

    private void DrawBG(SpriteBatch spriteBatch)
    {
        if (!ShowBG) return;
        menuBackground.Draw(spriteBatch);
    }

    #endregion Scene
}