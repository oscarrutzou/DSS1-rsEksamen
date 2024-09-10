using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.Weapons.MeleeWeapons;
using ShamansDungeon.ComponentPattern.Weapons.RangedWeapons;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShamansDungeon.Factory;

public enum WeaponTypes
{
    Sword,
    Axe,
    Dagger,
    MagicStaff,
    MagicStaffFire,
    Bow,
    BowFire,
    WoodArrow,
}

// Erik
public static class WeaponFactory
{
    public static List<WeaponTypes> WeaponTypesThatAreDone { get; private set; } = new()
    {
        WeaponTypes.Sword,
        WeaponTypes.Dagger,
        WeaponTypes.Axe,
        //WeaponTypes.MagicStaff,
    };

    public static Dictionary<ClassTypes, List<WeaponTypes>> ClassHasWeapons { get; private set; } = new()
    {
        { ClassTypes.Assassin, new List<WeaponTypes>(){
            WeaponTypes.Dagger,
            WeaponTypes.Bow,
            WeaponTypes.BowFire,

            //WeaponTypes.MagicStaff,

        }},

        { ClassTypes.Mage, new List<WeaponTypes>(){
            //WeaponTypes.MagicStaff, // Right now it only lets you add one weapon that are unique. Change that
            WeaponTypes.MagicStaffFire,
        }},

        { ClassTypes.Warrior, new List<WeaponTypes>(){
            WeaponTypes.Sword,
            WeaponTypes.Axe,
        }},
    };

    public static Dictionary<EnemyTypes, List<WeaponTypes>> EnemyHasWeapon { get; private set; } = new()
    {
        { EnemyTypes.OrcArcher, new List<WeaponTypes>(){
            WeaponTypes.Dagger,
            //WeaponTypes.Bow,
            //WeaponTypes.BowFire,
        }},

        { EnemyTypes.OrcShaman, new List<WeaponTypes>(){
            WeaponTypes.MagicStaff,
            //WeaponTypes.MagicStaffFire,
        }},

        { EnemyTypes.OrcWarrior, new List<WeaponTypes>(){
            WeaponTypes.Sword,
            //WeaponTypes.Axe,
        }},
    };

    public static GameObject Create(WeaponTypes type)
    {
        GameObject weaponGo = new();
        weaponGo.Type = GameObjectTypes.Weapon;
        weaponGo.AddComponent<SpriteRenderer>();
        AddClassComponent(weaponGo, type);

        return weaponGo;
    }
    private static void AddClassComponent(GameObject weaponGo, WeaponTypes type)
    {
        switch (type)
        {
            case WeaponTypes.Sword:
                weaponGo.AddComponent<Sword>();
                break;

            case WeaponTypes.Axe:
                weaponGo.AddComponent<Axe>();
                break;

            case WeaponTypes.Dagger:
                weaponGo.AddComponent<Dagger>();
                break;

            case WeaponTypes.MagicStaff:
                weaponGo.AddComponent<MagicStaff>();
                break;

            case WeaponTypes.MagicStaffFire:
                weaponGo.AddComponent<MagicStaffFire>();
                break;

            case WeaponTypes.Bow:
                weaponGo.AddComponent<Bow>();
                break;

            case WeaponTypes.BowFire:
                weaponGo.AddComponent<BowFire>();
                break;
        }
    }
}