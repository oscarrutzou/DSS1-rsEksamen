using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.ComponentPattern.Weapons;
using FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.Factory
{
    internal class WeaponFactory : Factory
    {
        private Player player;
        public override GameObject Create()
        {
            GameObject sword = new GameObject();
            sword.Transform.Scale = new Vector2(3,8);
            
            SpriteRenderer sr = sword.AddComponent<SpriteRenderer>();
            sr.SetSprite(TextureNames.Cell);
            sr.SetLayerDepth(LAYERDEPTH.Player);
            sword.AddComponent<Collider>();
            sword.AddComponent<MagicStaff>();

            return sword;
        }

        public  GameObject Create(GameObject playerGo)
        {
            GameObject sword = new GameObject();
            sword.Transform.Scale = new Vector2(3, 8);
            sword.Transform.Position = playerGo.Transform.Position;
            SpriteRenderer sr = sword.AddComponent<SpriteRenderer>();
            sr.SetSprite(TextureNames.Cell);
            sr.SetLayerDepth(LAYERDEPTH.Player);
            sword.AddComponent<Collider>();
            sword.AddComponent<MagicStaff>();

            return sword;
        }
    }
}
