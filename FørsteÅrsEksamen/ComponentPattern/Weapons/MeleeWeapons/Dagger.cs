﻿using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons;

public class Dagger : MeleeWeapon
{
    private static SoundNames[] _daggerAttackSounds = new SoundNames[]
    {
        //SoundNames.SwipeFast1,
        //SoundNames.SwipeFast2,
        //SoundNames.SwipeFast3,
        SoundNames.SwipeFast4,
        SoundNames.SwipeFast5,
    };

    private static Dictionary<WeaponAnimTypes, WeaponAnimation> _daggerAnimations = new ()
    {
        { WeaponAnimTypes.Light, new WeaponAnimation(0.4f, MathHelper.PiOver4* 3, 15, BaseMath.EaseOutQuart, WeaponAnimTypes.Medium, 2)},
        { WeaponAnimTypes.Medium, new WeaponAnimation(0.7f, MathHelper.PiOver4* 6, 30, BaseMath.EaseInOutQuint, WeaponAnimTypes.Light)},
    };


    public Dagger(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Start()
    {
        AttackSoundNames = _daggerAttackSounds;
        Animations = _daggerAnimations;

        CurrentAnim = WeaponAnimTypes.Light;

        MinimumTimeBetweenHits = 0.2f;

        if (EnemyUser != null)
        {
            EnemyUser.CellPlayerMoveBeforeNewTarget = 2;
        }
        
        base.Start();
    }

    protected override void PlayerWeaponSprite()
    {
        SpriteRenderer.SetSprite(TextureNames.WoodDagger);
        SetStartColliders(new Vector2(7.5f, 21), 5, 5, 2, 3); // Gets set in each of the weapons insted of here.
    }

    protected override void EnemyWeaponSprite()
    {
        SpriteRenderer.SetSprite(TextureNames.BoneDagger);
        SetStartColliders(new Vector2(7.5f, 21), 5, 5, 2, 3); // Gets set in each of the weapons insted of here.
    }
}