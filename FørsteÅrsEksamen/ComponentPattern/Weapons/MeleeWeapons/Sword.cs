using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.ComponentPattern.Weapons.MeleeWeapons
{
    internal class Sword : MeleeWeapon
    {
        public Sword(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Start()
        {
            AttackSoundNames = new SoundNames[]
            {
                SoundNames.SwipeSlow1,
            };
            spriteRenderer.SetSprite(TextureNames.WoodSword);
            SetStartColliders(new Vector2(7.5f, 38), 5, 5, 6, 4); // Gets set in each of the weapons insted of here.

        }

        public override void Attack()
        {
            base.Attack();
        }
    }
}