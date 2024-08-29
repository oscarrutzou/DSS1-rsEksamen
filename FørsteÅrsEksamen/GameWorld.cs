using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.GameManagement.Scenes;
using DoctorsDungeon.GameManagement.Scenes.Menus;
using DoctorsDungeon.GameManagement.Scenes.Rooms;
using DoctorsDungeon.GameManagement.Scenes.TestScenes;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DoctorsDungeon;

// Alle
public class GameWorld : Game
{
    #region Properties
    public static GameWorld Instance;

    public static bool DebugAndCheats = true;
    public static double DeltaTime { get; private set; }
    public static bool IsPaused = false;
    public static Color BackGroundColor { get; private set; } = new Color(20, 20, 18, 255);
    public static Color TextColor { get; private set; } = new Color(250, 249, 246);
    public GraphicsDeviceManager GfxManager { get; private set; } 
    public float AvgFPS { get; private set; }
    public Dictionary<SceneNames, Scene> Scenes { get; private set; }
    public Scene CurrentScene { get; private set; } 
    public Camera WorldCam { get; private set; } // Follows player
    public Camera UiCam { get; private set; } //Static on the ui
    public SceneNames? NextScene { get; private set; } = null;
    public bool ShowBG { get; set; } = true; // If we should show our background
     
    public int DisplayWidth => GfxManager.PreferredBackBufferWidth;
    public int DisplayHeight => GfxManager.PreferredBackBufferHeight;

    private SpriteBatch _spriteBatch;
    private readonly string _menuString = "Menu";
    public bool IsInMenu { get; private set; } = true;


    #region Shader Params
    public float HighlightsEffect_Threshold = 0.465f; // 0.25f shows a lot
    public float GaussianBlurEffect_BlurAmount = 5f;
    public float VignetteInner = 0.54f;
    public float VignetteOuter = 0.77f;

    public bool SingleColorEffect = false;
    public double GameWorldSpeed = 1.0f;
    private Canvas _canvas; // The canvas is a render target that handles the shader stuff
    #endregion
    #endregion

    public GameWorld()
    {
        GfxManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "A-Content";
        IsMouseVisible = false;
        Window.Title = "Doctor's Dungeon";
    }

    protected override void Initialize()
    {
        _canvas = new(GraphicsDevice);

        // Frametime not limited to 16.66 Hz / 60 FPS, and will drop if the mouse is outside the bounds
        IsFixedTimeStep = false;
        GfxManager.SynchronizeWithVerticalRetrace = true;

        // Put some of this into threads to load faster in a loading menu, insted of running it here.
        GlobalTextures.LoadContent();
        GlobalSounds.LoadContent();
        GlobalAnimations.LoadContent();

        Fullscreen(); // Need to be before the camera
        //SetResolutionSize(800, 800); 

        SceneData.Instance.GenereateGameObjectDicionary();
        
        WorldCam = new Camera(); // Camera that follows the player
        UiCam = new Camera();    // Camera that is static


        GenerateScenes(); // Makes a instance of all the scene we need

        CurrentScene = Scenes[SceneNames.MainMenu];
        CurrentScene.Initialize(); // Starts the main menu 
         
        IndependentBackground.SpawnBG(); // The background that dont get deleted

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        SetShaderParams();
    }

    protected override void Update(GameTime gameTime)
    {
        DeltaTime = gameTime.ElapsedGameTime.TotalSeconds * GameWorldSpeed;

        UpdateFPS(gameTime);

        IndependentBackground.Update();
        InputHandler.Instance.Update();

        if (!this.IsActive) return;

        GlobalSounds.MusicUpdate(); // Updates the Music in the game, not SFX

        CurrentScene.Update(); // Updates all gameobjects and their componetents in the scene
        HandleSceneChange(); // Goes to the next scene

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _canvas.Activate();

        CurrentScene.DrawSceenColor();

        // This spriteBatch is for drawing a non pixelart background
        _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
            SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
            transformMatrix: WorldCam.GetMatrix());
        IndependentBackground.DrawBG(_spriteBatch);
        _spriteBatch.End();

        CurrentScene.DrawInWorld(_spriteBatch);

        // Draws the screen with the effects on
        // We dont want our normal effects to show on the canvas, but we do want to show the single color effect 
        if (!SingleColorEffect)
            _canvas.Draw(_spriteBatch);

        //Draw on screen objects. Use pixel perfect and a stationary UiCam that dosent move around
        _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
            SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
            transformMatrix: UiCam.GetMatrix());

        CurrentScene.DrawOnScreen(_spriteBatch);
        //DrawDebugShaderStrings();
        _spriteBatch.End();

        if (SingleColorEffect)
            _canvas.Draw(_spriteBatch);

        
        base.Draw(gameTime);
    }

    private void SetShaderParams()
    {

        Vector4 color = new Vector4(TextColor.R, TextColor.G, TextColor.B, TextColor.A);
        GlobalTextures.SingleColorEffect.Parameters["singleColor"].SetValue(color);
        GlobalTextures.SingleColorEffect.Parameters["threshold"].SetValue(0.23f);

        GlobalTextures.HighlightsEffect.Parameters["threshold"].SetValue(HighlightsEffect_Threshold);
        GlobalTextures.GaussianBlurEffect.Parameters["blurAmount"].SetValue(GaussianBlurEffect_BlurAmount);

        GlobalTextures.GaussianBlurEffect.CurrentTechnique = GlobalTextures.GaussianBlurEffect.Techniques["Blur"]; // Basic ; Blur

        GlobalTextures.VignetteEffect.Parameters["innerRadius"].SetValue(VignetteInner);
        GlobalTextures.VignetteEffect.Parameters["outerRadius"].SetValue(VignetteOuter);
    }

    private void DrawDebugShaderStrings()
    {
        Vector2 pos = UiCam.LeftCenter;
        _spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Thredshold: {HighlightsEffect_Threshold}", pos, Color.White);
        pos += new Vector2(0, 30);
        _spriteBatch.DrawString(GlobalTextures.DefaultFont, $"BlurAmount: {GaussianBlurEffect_BlurAmount}", pos, Color.White);
        pos += new Vector2(0, 30);
        _spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Vignet Inner: {VignetteInner}", pos, Color.White);
        pos += new Vector2(0, 30);
        _spriteBatch.DrawString(GlobalTextures.DefaultFont, $"Vignet Outer: {VignetteOuter}", pos, Color.White);
        _spriteBatch.End();
    }

    public void SetResolutionSize(int width, int height)
    {
        GfxManager.HardwareModeSwitch = true;
        GfxManager.PreferredBackBufferWidth = width;
        GfxManager.PreferredBackBufferHeight = height;
        GfxManager.IsFullScreen = false;
        GfxManager.ApplyChanges();

        ScreenChanged();
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

        ScreenChanged();
    }

    private void ScreenChanged()
    {
        _canvas.SetDestinationRectangle();
    }

    private void UpdateFPS(GameTime gameTime)
    {
        float fps = 0;
        if (gameTime.ElapsedGameTime.TotalMilliseconds > 0)
            fps = (float)Math.Round(1000 / (gameTime.ElapsedGameTime.TotalMilliseconds), 1);

        //Set _avgFPS to the first fps value when started.
        if (AvgFPS < 0.01f) AvgFPS = fps;

        //Average over 20 frames
        AvgFPS = AvgFPS * 0.95f + fps * 0.05f;
    }

    #region Scene
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
            [SceneNames.DungeonRoom4] = new Room4Scene(),

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
        NextScene = sceneName;

        // Starts the background emitter if scene ends in Menu
        string name = sceneName.ToString();
        if (name.Length >= 4 && name[^4..] == _menuString) // name.Substring(name.Length - 4)
            IsInMenu = true;
        else
            IsInMenu = false;
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

            if (Scenes.ContainsKey(newScene)) // The next scene dosent exit
            {
                NextScene = newScene;
                IsInMenu = false;
            }
            else
            {
                NextScene = SceneNames.MainMenu; // Sends them back to the menu
                IsInMenu = true;
            }
        }
        else
        {
            NextScene = SceneNames.MainMenu; // Sends them back to the menu
            IsInMenu = true;
        }
    }

    /// <summary>
    /// A method to prevent changing in the GameObject lists while its still inside the Update
    /// </summary>
    private void HandleSceneChange()
    {
        if (NextScene == null || Scenes[NextScene.Value] == null) return;

        SingleColorEffect = false;

        if (!CurrentScene.IsChangingScene)
        {
            CurrentScene.StartSceneChange();
        }

        CurrentScene.OnSceneChange(); // Removes commands and more

        // Wait for current scene to turn down alpha on objects
        if (CurrentScene.IsChangingScene) return;
        
        SceneData.Instance.DeleteAllGameObjects(); 

        WorldCam.Position = Vector2.Zero; // Resets world cam position
        CurrentScene = Scenes[NextScene.Value]; // Changes to new scene
        CurrentScene.Initialize(); // Starts the new scene

        // Set color of all scene objects

        NextScene = null;
    }


    #endregion Scene
}


public class ShaderEffectData
{
    public RenderTarget2D RenderTarget { get; set; }
    public Effect ShaderEffect {  get; set; }

    public ShaderEffectData(GraphicsDevice graphicsDevice, int width, int height, Effect effect)
    {
        RenderTarget = new RenderTarget2D(graphicsDevice, width, height);
        ShaderEffect = effect;
    }
}

public class Canvas
{
    private RenderTarget2D _baseScreen;
    private readonly GraphicsDevice _graphicsDevice;
    private Rectangle _destinationRectangle;

    private enum ShaderEffectNames
    {
        // The name for each effect
        Bloom,
        SingleColor,
    }
    // The list should be in a correct rækkefølge
    private Dictionary<ShaderEffectNames, List<ShaderEffectData>> ShaderEffects = new();
    public Canvas(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    private void AddBloom()
    {
        int width = GameWorld.Instance.DisplayWidth;
        int height = GameWorld.Instance.DisplayHeight;

        if (ShaderEffects.ContainsKey(ShaderEffectNames.Bloom)) ShaderEffects.Remove(ShaderEffectNames.Bloom);

        ShaderEffects.Add(ShaderEffectNames.Bloom, new List<ShaderEffectData>()
        {
            { new ShaderEffectData(_graphicsDevice, width, height, GlobalTextures.HighlightsEffect)  },
            { new ShaderEffectData(_graphicsDevice, width, height, GlobalTextures.GaussianBlurEffect)  },
        });
    }

    /// <summary>
    /// This is so we can set the base game to be a smaller scale and then just upscale everything. 
    /// <para>Does nothing right now</para>
    /// </summary>
    public void SetDestinationRectangle()
    {
        int width = GameWorld.Instance.DisplayWidth;
        int height = GameWorld.Instance.DisplayHeight;

        _baseScreen = new(_graphicsDevice, width, height);

        // Makes a new shader list
        AddBloom();

        var screenSize = _graphicsDevice.PresentationParameters.Bounds;
        float scaleX = (float)screenSize.Width / _baseScreen.Width;
        float scaleY = (float)screenSize.Height / _baseScreen.Height;
        float scale = Math.Min(scaleX, scaleY);

        int newWidth = (int)(_baseScreen.Width * scale);
        int newHeight = (int)(_baseScreen.Height * scale);

        int posX = (screenSize.Width - newWidth) / 2;
        int posY = (screenSize.Height - newHeight) / 2;

        _destinationRectangle = new Rectangle(0, 0, newWidth, newHeight);
    }

    public void Activate()
    {
        _graphicsDevice.SetRenderTarget(_baseScreen);
    }

    // Make this so it can be split up.
    // Then take some of the part and use with the normal object,
    // and another for the bg. Then the bg can be drawn with its own shader.
    // Then we can use the gameobject layer to draw out what we like with a sprite offset, or dissolver.
    public void Draw(SpriteBatch spriteBatch)
    {
        // Everthing drawn before this, will be used for the effect

        Texture2D finnishedScene = _baseScreen;
        
        foreach (var shaderEffectPair in ShaderEffects)
        {
            Texture2D baseScene = _baseScreen;

            ShaderEffectNames shaderEffectName = shaderEffectPair.Key;

            // Iterate over the inner dictionary (RenderTarget2D and Effect pairs)
            foreach (ShaderEffectData shaderEffectData in shaderEffectPair.Value)
            {
                RenderTarget2D renderTarget = shaderEffectData.RenderTarget;
                Effect shaderEFfect = shaderEffectData.ShaderEffect;

                _graphicsDevice.SetRenderTarget(renderTarget);

                spriteBatch.Begin(effect: shaderEFfect);
                spriteBatch.Draw(baseScene, Vector2.Zero, Color.White);
                spriteBatch.End();

                baseScene = renderTarget;
            }
        }

        // Reset the render target
        _graphicsDevice.SetRenderTarget(null);

        // Clear the screen
        _graphicsDevice.Clear(Color.Transparent);

        if (GameWorld.Instance.SingleColorEffect)
            DrawBaseScreen(spriteBatch, GlobalTextures.SingleColorEffect); // Need to make this effect also contain vignette
        else
            DrawBaseScreen(spriteBatch, GlobalTextures.VignetteEffect);

        // Draw the rest of the effects (All are going to be having chromatic aberration on them
        spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, effect: GameWorld.Instance.SingleColorEffect ? null : GlobalTextures.ChromaticAberrationEffect);

        foreach (var shaderEffectPair in ShaderEffects)
        {
            ShaderEffectNames shaderEffectName = shaderEffectPair.Key;

            // The last has the final effect
            RenderTarget2D shaderTarget = shaderEffectPair.Value.Last().RenderTarget;
            spriteBatch.Draw(shaderTarget, Vector2.Zero, Color.White); // Draw last of the effect
        }

        spriteBatch.End();
    }

    private void DrawBaseScreen(SpriteBatch spriteBatch, Effect effect)
    {
        spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, effect: effect);
        spriteBatch.Draw(_baseScreen, Vector2.Zero, Color.White);
        spriteBatch.End();
    }
    
}
