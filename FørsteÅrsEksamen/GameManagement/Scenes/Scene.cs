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

    WeaponTestScene,

    OscarTestScene,
    StefanTestScene,
    ErikTestScene,
    AsserTestScene,
}

// Oscar
public abstract class Scene
{
    private List<GameObject> newGameObjects = new List<GameObject>();
    private List<GameObject> destroyedGameObjects = new List<GameObject>();
    protected Action OnFirstCleanUp { get; set; }
    public bool IsChangingScene { get; set; }

    protected Color CurrentTextColor { get; set; }
    public double TransitionProgress { get; private set; }
    private double transitionDuration = 0.3; // Desired duration in seconds

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

        foreach (GameObjectTypes type in SceneData.GameObjectLists.Keys)
        {
            foreach (GameObject gameObject in SceneData.GameObjectLists[type])
            {
                gameObject.Update();
            }
        }
    }

    public void Instantiate(GameObject gameObject)
    {
        newGameObjects.Add(gameObject);
    }

    public void Destroy(GameObject go)
    {
        destroyedGameObjects.Add(go);
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
        TransitionProgress += GameWorld.DeltaTime / transitionDuration;
        TransitionProgress = Math.Clamp(TransitionProgress, 0, 1);

        foreach (GameObjectTypes type in SceneData.GameObjectLists.Keys)
        {
            // The cells shouldnt be turned transparent, since that allows the player to see whats under them.
            if (type == GameObjectTypes.Cell) continue; 

            foreach (GameObject gameObject in SceneData.GameObjectLists[type])
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
        if (newGameObjects.Count == 0 && destroyedGameObjects.Count == 0) return; //Shouldnt run since there is no new changes

        for (int i = 0; i < newGameObjects.Count; i++)
        {
            AddToCategory(newGameObjects[i]);
            newGameObjects[i].Awake();
            newGameObjects[i].Start();
        }
        for (int i = 0; i < destroyedGameObjects.Count; i++)
        {
            RemoveFromCategory(destroyedGameObjects[i]);
        }

        newGameObjects.Clear();
        destroyedGameObjects.Clear();
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
        SceneData.GameObjectLists[gameObject.Type].Add(gameObject);
    }

    /// <summary>
    /// Removes the gameObject from the respective list that they are in.
    /// </summary>
    /// <param name="gameObject"></param>
    private void RemoveFromCategory(GameObject gameObject)
    {
        SceneData.GameObjectLists[gameObject.Type].Remove(gameObject);
    }

    /// <summary>
    /// Draws everything that is not the Gui GameObjectType with the WorldCam.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public virtual void DrawInWorld(SpriteBatch spriteBatch)
    {
        foreach (GameObjectTypes type in SceneData.GameObjectLists.Keys)
        {
            if (type == GameObjectTypes.Gui) continue; //Skip GUI list

            foreach (GameObject gameObject in SceneData.GameObjectLists[type])
            {
                gameObject.Draw(spriteBatch);
            }
        }
    }

    /// <summary>
    /// Draws the Gui GameObjects on the UiCam.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public virtual void DrawOnScreen(SpriteBatch spriteBatch)
    {
        // Draw all Gui GameObjects in the active scene.
        foreach (GameObject gameObject in SceneData.GameObjectLists[GameObjectTypes.Gui])
        {
            gameObject.Draw(spriteBatch);
        }
    }

    public virtual void DrawSceenColor()
    {
        GameWorld.Instance.GraphicsDevice.Clear(GameWorld.BackGroundColor);
    }
}