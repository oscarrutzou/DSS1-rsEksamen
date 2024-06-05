using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.GameManagement.Scenes
{
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
        protected Action OnFirstCleanUp;

        public abstract void Initialize();

        /// <summary>
        /// The base update on the scene handles all the GameObjects and calls Update on them all.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            CleanUp();

            if (OnFirstCleanUp != null)
            {
                OnFirstCleanUp();
                OnFirstCleanUp = null;
            }

            if (GameWorld.IsPaused) return;

            foreach (GameObjectTypes type in SceneData.GameObjectLists.Keys)
            {
                foreach (GameObject gameObject in SceneData.GameObjectLists[type])
                {
                    gameObject.Update(gameTime);
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

        public virtual void OnSceneChange()
        {
            OnFirstCleanUp = null; // For extra safety
            InputHandler.Instance.RemoveAllExeptBaseCommands();
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
            GameWorld.Instance.GraphicsDevice.Clear(new Color(20, 20, 18, 255));
        }
    }
}