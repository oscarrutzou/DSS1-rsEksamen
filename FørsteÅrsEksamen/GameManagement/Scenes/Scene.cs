using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Particles;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.GameManagement.Scenes.Menus;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.GameManagement.Scenes;

public enum SceneNames
{
    MainMenu,
    SaveFileMenu,
    CharacterSelectorMenu,
    LoadingScreen,
    EndMenu,

    DungeonRoom, // BASE, NO SCRIPT HERE
    DungeonRoom1,
    DungeonRoom2,
    DungeonRoom3,
    DungeonRoom4,

    WeaponTestScene,

    OscarTestScene,
    StefanTestScene,
    ErikTestScene,
    AsserTestScene,
}

// Oscar
public abstract class Scene
{
    private List<GameObject> _newGameObjects = new List<GameObject>();
    private List<GameObject> _destroyedGameObjects = new List<GameObject>();
    protected Action OnFirstCleanUp { get; set; }
    public bool IsChangingScene { get; set; }

    protected Color CurrentTextColor { get; set; }
    public double TransitionProgress { get; private set; }
    private double _transitionDuration = 0.3; // Desired duration in seconds

    public abstract void Initialize();

    /// <summary>
    /// The base update on the scene handles all the GameObjects and calls Update on them all.
    /// </summary>
    public virtual void Update()
    {
        LerpTextColor();

        CleanUp();

        if (OnFirstCleanUp != null)
        {
            OnFirstCleanUp.Invoke();
            OnFirstCleanUp = null;
        }

        if (GameWorld.IsPaused) return;

        foreach (GameObjectTypes type in SceneData.Instance.GameObjectLists.Keys)
        {
            foreach (GameObject gameObject in SceneData.Instance.GameObjectLists[type])
            {
                gameObject.Update();
            }
        }
    }

    public void Instantiate(GameObject gameObject)
    {
        _newGameObjects.Add(gameObject);
    }

    public void Destroy(GameObject go)
    {
        _destroyedGameObjects.Add(go);
    }

    public virtual void OnPlayerChanged()
    { }

    public void StartSceneChange()
    {
        IsChangingScene = true;
        TransitionProgress = 0; // Start with full opacity
    }

    private void LerpGameObjects()
    {
        TransitionProgress += GameWorld.DeltaTime / _transitionDuration;
        TransitionProgress = Math.Clamp(TransitionProgress, 0, 1);

        foreach (GameObjectTypes type in SceneData.Instance.GameObjectLists.Keys)
        {
            // The cells shouldnt be turned transparent, since that allows the player to see whats under them.
            if (type == GameObjectTypes.Cell) continue; 

            foreach (GameObject gameObject in SceneData.Instance.GameObjectLists[type])
            {
                SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                if (sr.StartColor == Color.Transparent)
                    sr.StartColor = sr.Color;

                sr.Color = Color.Lerp(sr.StartColor, Color.Transparent, (float)TransitionProgress);
            }
        }

    }
    
    private void LerpTextColor()
    {
        CurrentTextColor = BaseMath.TransitionColor(GameWorld.TextColor);
    }

    public void OnSceneChange()
    {
        LerpGameObjects();

        // If the progrss of the lerp has not finnished, we wont change scenes
        if (TransitionProgress != 1) return;

        IsChangingScene = false;
        
        OnFirstCleanUp = null;

        InputHandler.Instance.RemoveAllExeptBaseCommands();
        GridManager.Instance.ResetGridManager();

        ParticleEmitter emitter = IndependentBackground.BackgroundEmitter;
        if (emitter == null) return;

        emitter.ResetFollowGameObject();
    }

    /// <summary>
    /// <para>The method adds the newGameobjects to different lists, and calls the Awake and Start on the Objects, so the objects starts properly.</para>
    /// <para>It also removes the gameobjects if there are any</para>
    /// </summary>
    private void CleanUp()
    {
        if (_newGameObjects.Count == 0 && _destroyedGameObjects.Count == 0) return; //Shouldnt run since there is no new changes

        for (int i = 0; i < _newGameObjects.Count; i++)
        {
            AddToCategory(_newGameObjects[i]);
            _newGameObjects[i].Awake();
            _newGameObjects[i].Start();
        }
        for (int i = 0; i < _destroyedGameObjects.Count; i++)
        {
            RemoveFromCategory(_destroyedGameObjects[i]);
        }

        _newGameObjects.Clear();
        _destroyedGameObjects.Clear();
    }

    /// <summary>
    /// Adds the gameObject to the correct list based on the gameobjects type.
    /// </summary>
    /// <param name="gameObject"></param>
    private void AddToCategory(GameObject gameObject)
    {
        // We know the Lists gets made and only gets Cleared when changing scene,
        // so we can assume that the lists are already there.
        // If something went wrong, the compiler will send a error anyway, so we dont need a extra check, for small projects like these.
        SceneData.Instance.GameObjectLists[gameObject.Type].Add(gameObject);

        // Checks if the gameobject has any effects on it.
        SceneData.Instance.AddGameObject(gameObject);
    }

    /// <summary>
    /// Removes the gameObject from the respective list that they are in.
    /// </summary>
    /// <param name="gameObject"></param>
    private void RemoveFromCategory(GameObject gameObject)
    {
        SceneData.Instance.GameObjectLists[gameObject.Type].Remove(gameObject);

        // Removes the gameobject if its in the list
        SceneData.Instance.RemoveGameObject(gameObject);
    }

    /// <summary>
    /// Draws everything that is not the Gui GameObjectType with the WorldCam.
    /// </summary>
    /// <param name="spriteBatch"></param>
    private Dictionary<Effect, List<GameObject>> effectGroups = new Dictionary<Effect, List<GameObject>>();

    public virtual void DrawInWorld(SpriteBatch spriteBatch)
    {
        bool usesSimpleDrawCall = false;

        foreach (var spriteRenderer in SceneData.Instance.GetSortedGameObjects())
        {
            if (spriteRenderer.GameObject.ShaderEffect == null)
            {
                if (!usesSimpleDrawCall)
                {
                    spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                        SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
                        transformMatrix: GameWorld.Instance.WorldCam.GetMatrix());
                    usesSimpleDrawCall = true;
                }
                spriteRenderer.GameObject.Draw(spriteBatch);
            }
            else
            {
                if (usesSimpleDrawCall)
                {
                    spriteBatch.End();
                    usesSimpleDrawCall = false;
                }
                spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                    SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
                    transformMatrix: GameWorld.Instance.WorldCam.GetMatrix(), effect: spriteRenderer.GameObject.ShaderEffect);
                spriteRenderer.GameObject.Draw(spriteBatch);
                spriteBatch.End();
            }
        }

        if (usesSimpleDrawCall) spriteBatch.End();
    }


    /// <summary>
    /// Draws the Gui GameObjects on the UiCam.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public virtual void DrawOnScreen(SpriteBatch spriteBatch)
    {
        DrawMouse(spriteBatch);

        // Draw all Gui GameObjects in the active scene.
        foreach (GameObject gameObject in SceneData.Instance.GameObjectLists[GameObjectTypes.Gui])
        {
            gameObject.Draw(spriteBatch);
        }
    }

    private void CheckForEffects(SpriteBatch spriteBatch)
    {
        spriteBatch.GraphicsDevice.GraphicsDebug.TryDequeueMessage(out _);
    }

    private void DrawMouse(SpriteBatch spriteBatch)
    {
        InputHandler.Instance.MouseGo?.Draw(spriteBatch);
        spriteBatch.DrawString(GlobalTextures.DefaultFont, $"FPS: {GameWorld.Instance.AvgFPS}", GameWorld.Instance.UiCam.TopLeft, Color.DarkGreen);
    }


    public virtual void DrawSceenColor()
    {
        GameWorld.Instance.GraphicsDevice.Clear(GameWorld.BackGroundColor);
    }
}