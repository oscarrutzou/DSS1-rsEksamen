using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons
{
    public class Dagger : MeleeWeapon
    {
        public Dagger(GameObject gameObject) : base(gameObject)
        {
            AttackSpeed = 3f;
            Damage = 17;
            LerpFromTo = MathHelper.Pi;
        }

        public override void Start()
        {
            AttackSoundNames = new SoundNames[]
            {
                SoundNames.SwipeFast1,
                SoundNames.SwipeFast2,
            };
        }

        protected override void PlayerWeaponSprite()
        {
            spriteRenderer.SetSprite(TextureNames.WoodDagger);
            SetStartColliders(new Vector2(7.5f, 21), 5, 5, 4, 3); // Gets set in each of the weapons insted of here.
        }

        protected override void EnemyWeaponSprite()
        {
            spriteRenderer.SetSprite(TextureNames.BoneDagger);
            SetStartColliders(new Vector2(7.5f, 21), 5, 5, 4, 3); // Gets set in each of the weapons insted of here.
        }
    }
}
