using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.LiteDB;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.GUI
{
    public class BackpackIcon : Component
    {
        public BackpackIcon(GameObject gameObject) : base(gameObject)
        {
        }
        private SpriteRenderer sr;

        private Player player;
        private Texture2D potionTexture;

        public override void Awake()
        {
            sr = GameObject.GetComponent<SpriteRenderer>();
            sr.SetSprite(TextureNames.BackpackIcon);

            player = SaveData.Player;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (player.ItemInInventory == null)
            {
                potionTexture = null;
                return;
            }
            if (potionTexture == null)
            {
                SpriteRenderer potionSr = player.ItemInInventory.GameObject.GetComponent<SpriteRenderer>();
                potionTexture = potionSr.Sprite;
            }
            // Draw the item inside the backpack icon

            sr.DrawSprite(spriteBatch, potionTexture, new Vector2(0, -10), 0.1f);
        }
    }
}
