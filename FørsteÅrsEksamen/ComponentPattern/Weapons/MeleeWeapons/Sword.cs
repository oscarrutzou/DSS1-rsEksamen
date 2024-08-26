using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons;

// Erik
public class Sword : MeleeWeapon
{
    private static SoundNames[] _swordAttackSounds = new SoundNames[]
    {
        SoundNames.SwipeHeavy1,
        SoundNames.SwipeHeavy2,
        SoundNames.SwipeHeavy3,
        //SoundNames.SwipeHeavy4,
    };

    private static Dictionary<WeaponAnimTypes, WeaponAnimation> _swordAnimations = new()
    {
        { WeaponAnimTypes.Light, new WeaponAnimation(0.7f, MathHelper.Pi, 25, BaseMath.EaseInOutBack, WeaponAnimTypes.Medium, 2)},
        { WeaponAnimTypes.Medium, new WeaponAnimation(1.0f, MathHelper.Pi + MathHelper.PiOver2, 40, BaseMath.EaseInOutBack, WeaponAnimTypes.Light, 1)},
    };

    public Sword(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Start()
    {
        AttackSoundNames = _swordAttackSounds;
        Animations = _swordAnimations;

        CurrentAnim = WeaponAnimTypes.Light;

        MinimumTimeBetweenHits = 0.3f;


        base.Start();
    }

    protected override void PlayerWeaponSprite()
    {
        SpriteRenderer.SetSprite(TextureNames.WoodSword);
        SetStartColliders(new Vector2(7.5f, 38), 5, 5, 0, 6); // Gets set in each of the weapons insted of here.
    }

    protected override void EnemyWeaponSprite()
    {
        SpriteRenderer.SetSprite(TextureNames.BoneSword);
        SetStartColliders(new Vector2(7.5f, 38), 5, 5, 0, 5); // Gets set in each of the weapons insted of here.
    }
}