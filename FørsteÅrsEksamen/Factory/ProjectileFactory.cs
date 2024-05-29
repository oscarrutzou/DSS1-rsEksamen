using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Weapons.RangedWeapons;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.Factory
{
    public static class ProjectileFactory
    {
        // Erik
        public static GameObject Create()
        {
            GameObject arrow = new GameObject();
            arrow.Transform.Scale = new Vector2(4, 4f);
            arrow.AddComponent<SpriteRenderer>();

            arrow.AddComponent<Projectile>();

            //arrow.AddComponent<Collider>().SetCollisionBox(30, 30);

            return arrow;
        }
    }
}