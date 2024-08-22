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
        private SpriteRenderer _sr;

        private Player _player;
        private Texture2D _potionTexture;

        public override void Awake()
        {
            _sr = GameObject.GetComponent<SpriteRenderer>();
            _sr.SetSprite(TextureNames.BackpackIcon);

            _player = SaveData.Player;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_player.ItemInInventory == null)
            {
                _potionTexture = null;
                return;
            }
            if (_potionTexture == null)
            {
                SpriteRenderer potionSr = _player.ItemInInventory.GameObject.GetComponent<SpriteRenderer>();
                _potionTexture = potionSr.Sprite;
            }
            // Draw the item inside the backpack icon

            _sr.DrawSprite(spriteBatch, _potionTexture, new Vector2(0, -10), 0.1f);
        }
    }
}
