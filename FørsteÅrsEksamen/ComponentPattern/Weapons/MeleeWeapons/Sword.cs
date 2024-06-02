using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons
{
    // Erik
    public class Sword : MeleeWeapon
    {
        public Sword(GameObject gameObject) : base(gameObject)
        {
            AttackSpeed = 1.7f;
            Damage = 25;
            LerpFromTo = MathHelper.Pi;
        }

        public override void Start()
        {
            AttackSoundNames = new SoundNames[]
            {
                SoundNames.SwipeSlow1,
            };
        }

        protected override void PlayerWeaponSprite()
        {
            spriteRenderer.SetSprite(TextureNames.BoneSword);
            SetStartColliders(new Vector2(7.5f, 38), 5, 5, 6, 4); // Gets set in each of the weapons insted of here.
        }

        protected override void EnemyWeaponSprite()
        {
            spriteRenderer.SetSprite(TextureNames.BoneSword);
            SetStartColliders(new Vector2(7.5f, 38), 5, 5, 6, 4); // Gets set in each of the weapons insted of here.
        }
    }
}