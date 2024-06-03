using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons;
using DoctorsDungeon.ComponentPattern.Weapons.RangedWeapons;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DoctorsDungeon.Factory
{
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
        public static Dictionary<ClassTypes, List<WeaponTypes>> ClassHasWeapons = new()
        {
            { ClassTypes.Archer, new List<WeaponTypes>(){
                WeaponTypes.Dagger,
                WeaponTypes.Bow,
                WeaponTypes.BowFire,
            }},

            { ClassTypes.Mage, new List<WeaponTypes>(){
                WeaponTypes.MagicStaff,
                WeaponTypes.MagicStaffFire,
            }},

            { ClassTypes.Warrior, new List<WeaponTypes>(){
                WeaponTypes.Sword,
                WeaponTypes.Axe,
            }},
        };

        public static Dictionary<EnemyTypes, List<WeaponTypes>> EnemyHasWeapon = new()
        {
            { EnemyTypes.OrcArcher, new List<WeaponTypes>(){
                WeaponTypes.Dagger,
                WeaponTypes.Bow,
                WeaponTypes.BowFire,
            }},

            { EnemyTypes.OrcShaman, new List<WeaponTypes>(){
                WeaponTypes.MagicStaff,
                WeaponTypes.MagicStaffFire,
            }},

            { EnemyTypes.OrcWarrior, new List<WeaponTypes>(){
                WeaponTypes.Sword,
                WeaponTypes.Axe,
            }},
        };

        public static GameObject Create(WeaponTypes type)
        {
            GameObject weaponGo = new();
            weaponGo.Type = GameObjectTypes.Weapon;
            weaponGo.Transform.Scale = new Vector2(4, 4);
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
                    weaponGo.AddComponent<MagicStaff>();
                    break;

                case WeaponTypes.Bow:
                    weaponGo.AddComponent<Bow>();
                    break;

                case WeaponTypes.BowFire:
                    weaponGo.AddComponent<MagicStaff>();
                    break;
            }
        }
    }
}