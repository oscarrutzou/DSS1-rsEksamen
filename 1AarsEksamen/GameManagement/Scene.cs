using FørsteÅrsEksamen.CompositPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.GameManagement
{
    public enum ScenesNames
    {
        //MainMenu,
        //LoadingScreen,
        GameScene,

        OscarTestScene,
        StefanTestScene,
        ErikTestScene,
        AsserTestScene,
        //EndMenu,
    }

    // Oscar
    public abstract class Scene
    {
        public bool isPaused;

        private List<GameObject> newGameObjects = new List<GameObject>();
        private List<GameObject> destoroyedGameObjects = new List<GameObject>();

        public abstract void Initialize();

        /// <summary>
        /// The base update on the scene handles all the GameObjects and calls Update on them all.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            CleanUp();

            if (isPaused) return;

            foreach (GameObjectTypes type in SceneData.GameObjectLists.Keys)
            {
                foreach (GameObject gameObject in SceneData.GameObjectLists[type])
                {
                    gameObject.Update(gameTime);
                }
            }

            CheckCollision();
        }

        public void Instantiate(GameObject gameObject) => newGameObjects.Add(gameObject);

        public void Destroy(GameObject go) => destoroyedGameObjects.Add(go);

        /// <summary>
        /// <para>The method adds the newGameobjects to different lists, and calls the Awake and Start on the Objects, so the objects starts properly.</para>
        /// <para>It also removes the gameobjects if there are any</para>
        /// </summary>
        private void CleanUp()
        {
            if (newGameObjects.Count == 0 && destoroyedGameObjects.Count == 0) return; //Shouldnt run since there is no new changes

            for (int i = 0; i < newGameObjects.Count; i++)
            {
                AddToCategory(newGameObjects[i]);
                newGameObjects[i].Awake();
                newGameObjects[i].Start();
            }
            for (int i = 0; i < destoroyedGameObjects.Count; i++)
            {
                RemoveFromCategory(destoroyedGameObjects[i]);
            }

            newGameObjects.Clear();
            destoroyedGameObjects.Clear();
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
            GameWorld.Instance.GraphicsDevice.Clear(Color.Beige);
        }

        public void CheckCollision()
        {
            //foreach (GameObject go1 in SceneData.GameObjects)
            //{
            //    foreach (GameObject go2 in SceneData.GameObjects)
            //    {
            //        if (go1 == go2) continue;
            //        //Dosent check between enemies
            //        //Enemy enemy1 = go1.GetComponent<Enemy>();
            //        //Enemy enemy2 = go2.GetComponent<Enemy>();
            //        //if (enemy1 != null && enemy2 != null) continue; //Shouldnt make collisions between 2 enemies.

            //        Collider col1 = go1.GetComponent<Collider>();
            //        Collider col2 = go2.GetComponent<Collider>();

            //        //Check base collisionbox
            //        if (col1 != null && col2 != null && col1.CollisionBox.Intersects(col2.CollisionBox))
            //        {
            //            go1.OnCollisionEnter(col2);
            //            go2.OnCollisionEnter(col1);
            //        }
            //    }
            //}
        }
    }
}