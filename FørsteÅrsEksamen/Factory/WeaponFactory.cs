using FørsteÅrsEksamen.ComponentPattern;
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
        public override GameObject Create()
        {
            GameObject sword = new GameObject();
            sword.Transform.Scale = new Vector2(4,4);
            SpriteRenderer sr = sword.AddComponent<SpriteRenderer>();
            sr.SetSprite(TextureNames.WoodSword);
            sr.SetLayerDepth(LAYERDEPTH.Player);
            sword.AddComponent<MagicStaff>();

            return sword;
        }
    }
}
