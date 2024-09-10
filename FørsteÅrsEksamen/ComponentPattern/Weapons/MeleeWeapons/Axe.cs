using Microsoft.Xna.Framework;
using ShamansDungeon.GameManagement;
using ShamansDungeon.Other;
using System.Collections.Generic;

namespace ShamansDungeon.ComponentPattern.Weapons.MeleeWeapons;

// Erik
public class Axe : MeleeWeapon
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
        { WeaponAnimTypes.Light, new WeaponAnimation(1.0f, MathHelper.Pi, 35, BaseMath.EaseInOutBack, WeaponAnimTypes.Medium, 2)},
        { WeaponAnimTypes.Medium, new WeaponAnimation(1.5f, MathHelper.Pi + MathHelper.PiOver2, 50, BaseMath.EaseInOutBack, WeaponAnimTypes.Light, 1)},
    };

    public Axe(GameObject gameObject) : base(gameObject)
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
        SpriteRenderer.SetSprite(TextureNames.WoodAxe);
        SetStartColliders(new Vector2(7.5f, 23), 4, 6, 8, 2); // Gets set in each of the weapons insted of here.
    }

    protected override void EnemyWeaponSprite()
    {
        SpriteRenderer.SetSprite(TextureNames.BoneAxe);
        SetStartColliders(new Vector2(7.5f, 23), 3, 6, 8, 2); // Gets set in each of the weapons insted of here.
    }
}