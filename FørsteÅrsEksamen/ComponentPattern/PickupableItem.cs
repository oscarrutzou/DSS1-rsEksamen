using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using System;

namespace FørsteÅrsEksamen.ComponentPattern
{
    //Asser
    public class PickupableItem : Component
    {
        protected SpriteRenderer SpriteRenderer;
        protected Collider Collider, PlayerCollider;
        protected Player Player;

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
            Collider = GameObject.GetComponent<Collider>();
            Collider.SetCollisionBox(12, 19, new Vector2(2, 2));
            SpriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            PlayerCollider = playerGo.GetComponent<Collider>();
            Player = playerGo.GetComponent<Player>();
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnCollisionEnter(Collider collider)
        {
            // Skal kun fjerne item ved player position, ikke alle items.
            if (collider.CollisionBox.Intersects(PlayerCollider.CollisionBox))
            {
                Player.PickUpItem(GameObject);
                GameWorld.Instance.Destroy(GameObject);
            }
        }

        public void Use()
        {
            if (Player.CurrentHealth < Player.MaxHealth)
            {
                Player.CurrentHealth += 50;
                if (Player.CurrentHealth > Player.MaxHealth)
                {
                    Player.CurrentHealth = 100;
                }
            }
            else if (Player.CurrentHealth == Player.MaxHealth)
            {
                throw new Exception("I already have full health, it would be a waste to drink this now");
            }
        }
    }
}