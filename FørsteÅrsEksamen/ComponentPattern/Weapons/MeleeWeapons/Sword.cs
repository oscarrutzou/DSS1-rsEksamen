using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons;

// Erik
public class Sword : MeleeWeapon
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

        Animations = new()
        {
            { WeaponAnimTypes.Light, new WeaponAnimation(1.5f, MathHelper.Pi, 25, BaseMath.EaseInOutBack, WeaponAnimTypes.Light)},
        };

        CurrentAnim = WeaponAnimTypes.Light;
    }

    protected override void PlayerWeaponSprite()
    {
        spriteRenderer.SetSprite(TextureNames.WoodSword);
        SetStartColliders(new Vector2(7.5f, 38), 5, 5, 6, 4); // Gets set in each of the weapons insted of here.
    }

    protected override void EnemyWeaponSprite()
    {
        spriteRenderer.SetSprite(TextureNames.BoneSword);
        SetStartColliders(new Vector2(7.5f, 38), 5, 5, 6, 4); // Gets set in each of the weapons insted of here.
    }
}