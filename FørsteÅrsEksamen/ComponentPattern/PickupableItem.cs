using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using System;

namespace FørsteÅrsEksamen.ComponentPattern
{
    //Asser
    public class PickupableItem : Component
    {
        internal SpriteRenderer spriteRenderer;
        internal Collider collider, playerCollider;
        internal Player player;

        private GameObject playerGo;

        public string Name = "Health Potion";

        public PickupableItem(GameObject gameObject) : base(gameObject)
        {
        }

        public PickupableItem(GameObject gameObject, GameObject player) : base(gameObject)
        {
            playerGo = player;
        }

        public override void Awake()
        {
            base.Awake();
            collider = GameObject.GetComponent<Collider>();
            collider.SetCollisionBox(12, 19, new Vector2(2, 2));
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            playerCollider = playerGo.GetComponent<Collider>();
            player = playerGo.GetComponent<Player>();
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnCollisionEnter(Collider collider)
        {
            // Skal kun fjerne item ved player position, ikke alle items.
            if (collider.CollisionBox.Intersects(playerCollider.CollisionBox))
            {
                player.PickUpItem(GameObject);
                GameWorld.Instance.Destroy(GameObject);
            }
        }

        public void Use()
        {
            if (player.CurrentHealth < player.MaxHealth)
            {
                player.CurrentHealth += 50;
                if (player.CurrentHealth > player.MaxHealth)
                {
                    player.CurrentHealth = 100;
                }
            }
            else if (player.CurrentHealth == player.MaxHealth)
            {
                throw new Exception("I already have full health, it would be a waste to drink this now");
            }
        }
    }
}