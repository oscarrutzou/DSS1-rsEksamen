using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Weapons.RangedWeapons;

public class MagicStaff : RangedWeapon
{
    private static SoundNames[] _swordAttackSounds = new SoundNames[]
    {
        SoundNames.SwipeHeavy1,
        SoundNames.SwipeHeavy2,
        SoundNames.SwipeHeavy3,
        //SoundNames.SwipeHeavy4,
    };

    public MagicStaff(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Start()
    {
        AttackSoundNames = _swordAttackSounds;

        base.Start();
    }

    protected override void PlayerWeaponSprite()
    {
        SpriteRenderer.SetSprite(TextureNames.BoneMagicStaff);
        SpriteRenderer.SetOriginOffset(new Vector2(7.5f, 30));
    }

    protected override void EnemyWeaponSprite()
    {
        SpriteRenderer.SetSprite(TextureNames.BoneMagicStaff);
        SpriteRenderer.SetOriginOffset(new Vector2(7.5f, 30));

    }
}