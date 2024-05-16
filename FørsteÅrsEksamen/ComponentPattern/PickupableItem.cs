using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.ComponentPattern
{
    //Asser
    public class PickupableItem : Component
    {
        internal SpriteRenderer spriteRenderer;
        internal Collider collider;
        
        private readonly float threshold = 20f;
        private Vector2 position;
        private Point distanceToPlayer;
        private GameObject playerGo;
        private List<GameObject> itemsOnGround = new List<GameObject>();

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

        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void OnCollisionEnter(Collider collider)
        {

            // Skal kun fjerne item ved player position, ikke alle items.
            if (playerGo.CollidesWithGameObject(GameObjectTypes.Items))
            {
                //itemsOnGround.Remove(itemAtPlayerPosition);
            }
            
        }

     
    }
}
