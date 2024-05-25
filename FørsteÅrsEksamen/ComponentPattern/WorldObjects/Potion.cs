using FørsteÅrsEksamen.ComponentPattern.PlayerClasses;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using System;

namespace FørsteÅrsEksamen.ComponentPattern.WorldObjects
{
    //Asser
    public class Potion : Component
    {
        protected SpriteRenderer SpriteRenderer;
        protected Collider Collider, PlayerCollider;
        protected Player Player;

        private GameObject playerGo;

        public string Name = "Health Potion";
        private int healAmount = 50;

        public Potion(GameObject gameObject) : base(gameObject)
        {
        }

        public Potion(GameObject gameObject, GameObject player) : base(gameObject)
        {
            playerGo = player;
        }

        public override void Awake()
        {
            base.Awake();
            Collider = GameObject.GetComponent<Collider>();
            Collider.SetCollisionBox(10, 15);
            SpriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            PlayerCollider = playerGo.GetComponent<Collider>();
            Player = playerGo.GetComponent<Player>();
        }

        public override void Update(GameTime gameTime)
        {
            OnCollisionEnter(Collider);
        }

        public override void OnCollisionEnter(Collider collider)
        {
            // Skal kun fjerne item ved player position, ikke alle items.
            if (collider.CollisionBox.Intersects(PlayerCollider.CollisionBox))
            {
                Player.PickUpItem(this);
                GameObject.IsEnabled = false;
            }
        }

        public void Use()
        {
            if (Player.CurrentHealth == Player.MaxHealth) return;

            Player.CurrentHealth += healAmount;
            
            if (Player.CurrentHealth > Player.MaxHealth)
            {
                Player.CurrentHealth = 100;
            }

            Player.ItemInInventory = null;
            GameWorld.Instance.Destroy(GameObject);
        }
    }
}