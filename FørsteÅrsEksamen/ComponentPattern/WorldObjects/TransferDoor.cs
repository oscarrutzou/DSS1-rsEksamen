﻿using DoctorsDungeon.LiteDB;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.WorldObjects
{
    public class TransferDoor : Component
    {
        private Collider collider, playerCollider;
        private float timer;
        private float timeTillActivation = 2f;

        // Open sprite
        // Close dont draw sprite:)
        public bool CanTranser;

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
            if (!CanTranser) return; //Stop the check from happening if the player hasent killed all enemies.
            // Skal kun fjerne item ved player position, ikke alle items.
            if (collider.CollisionBox.Intersects(playerCollider.CollisionBox))
            {
                timer += GameWorld.DeltaTime;

                // Darken screen (Remeber to brighten screen if player goes out of the collision box

                if (timer >= timeTillActivation)
                {
                    DBMethods.CheckChangeDungeonScene();
                    timer = 0f;
                }
            }
        }
    }
}