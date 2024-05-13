using FørsteÅrsEksamen.ComponentPattern.Characters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
        private Vector2 position, distanceToPlayer;
        private GameObject playerGo;
        
        public PickupableItem(GameObject gameObject, GameObject player, Vector2 position) : base(gameObject)
        {
            playerGo = player;
        }

        public override void Awake()
        {
            base.Awake();
            collider.SetCollisionBox(12, 19, new Vector2(2, 2));
        }

        public void InPickUpRange()
        {
            collider.CollisionBox.Intersects(playerGo.GetComponent<Collider>(collider.CollisionBox));
        }
    }
}
