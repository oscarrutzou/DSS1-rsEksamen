using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.Factory
{
    internal class ProjectileFactory : Factory
    {
        public override GameObject Create()
        {
            GameObject arrow = new GameObject();
            arrow.Transform.Scale = new Vector2(0.4f, 0.4f);
            SpriteRenderer sr = arrow.AddComponent<SpriteRenderer>();
            sr.SetSprite(GameManagement.TextureNames.Cell);
            sr.SetLayerDepth(LAYERDEPTH.Player);
            arrow.AddComponent<Collider>().SetCollisionBox(30, 30);
            arrow.AddComponent<MagicStaff>();

            return arrow;
        }
    }
}