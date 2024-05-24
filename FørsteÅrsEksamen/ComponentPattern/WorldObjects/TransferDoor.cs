using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.DB;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.ComponentPattern.WorldObjects
{
    public class TransferDoor : Component
    {
        private Collider collider, playerCollider;
        private float timer;
        private float timeTillActivation = 2f;


        public TransferDoor(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Start()
        {
            playerCollider = SaveData.Player.GameObject.GetComponent<Collider>();
            collider = GameObject.GetComponent<Collider>();
        }

        public override void Update(GameTime gameTime)
        {
            OnCollisionEnter(collider);
        }

        public override void OnCollisionEnter(Collider collider)
        {
            // Skal kun fjerne item ved player position, ikke alle items.
            if (collider.CollisionBox.Intersects(playerCollider.CollisionBox))
            {
                timer += GameWorld.DeltaTime;

                // Darken screen (Remeber to brighten screen if player goes out of the collision box

                if (timer >= timeTillActivation)
                {
                    OnCollision();
                    timer = 0f;
                }
            }
        }

        private async void OnCollision()
        {
            // After it has saved the palyer it will change scene
            await Task.Run(DBMethods.SavePlayer);

            int newRoomNr = SaveData.Room_Reached + 1;
            GameWorld.Instance.ChangeDungounScene(SceneNames.DungounRoom, newRoomNr);
        }
    }
}
