using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.Weapons.RangedWeapons
{
    public class Arrow : Projectile
    {
        private Weapon weapon;
        public Arrow(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Start()
        {
            //SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>();
            //sr.SetSprite(TextureNames.WoodArrow);
            ////weapon.SetStartColliders(new Vector2(7.5f, 38), 5, 5, 6, 4);
        }
    }
}
