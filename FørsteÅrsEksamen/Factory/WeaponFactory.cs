using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.PlayerClasses;
using FørsteÅrsEksamen.ComponentPattern.Weapons.MeleeWeapons;
using FørsteÅrsEksamen.ComponentPattern.Weapons.RangedWeapons;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.Factory
{
    public enum WeaponTypes
    {
        Sword,
        Axe,
        MagicStaff,
        MagicStaffFire,
        Bow,
        BowFire,
        WoodArrow,
    }

    public static class WeaponFactory
    {
        public static Dictionary<ClassTypes, List<WeaponTypes>> ClassHasWeapons = new()
        {
            { ClassTypes.Archer, new List<WeaponTypes>(){
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

        public static GameObject Create(WeaponTypes type, bool enemyWeapon)
        {
            GameObject weaponGo = new GameObject();
            weaponGo.Type = GameObjectTypes.Weapon;
            weaponGo.Transform.Scale = new Vector2(4, 4);
            weaponGo.AddComponent<SpriteRenderer>();
            AddClassComponent(weaponGo, type, enemyWeapon);

            return weaponGo;
        }

        private static GameObject AddClassComponent(GameObject weaponGo, WeaponTypes type, bool enemyWeapon)
        {
            switch (type)
            {
                case WeaponTypes.Sword:
                    weaponGo.AddComponent<Sword>(enemyWeapon);
                    break;

                case WeaponTypes.Axe:
                    weaponGo.AddComponent<Axe>(enemyWeapon);
                    break;

                case WeaponTypes.MagicStaff:
                    weaponGo.AddComponent<MagicStaff>(enemyWeapon);
                    break;

                case WeaponTypes.MagicStaffFire:
                    weaponGo.AddComponent<MagicStaff>(enemyWeapon);
                    break;

                case WeaponTypes.Bow:
                    weaponGo.AddComponent<Bow>(enemyWeapon);
                    break;

                case WeaponTypes.BowFire:
                    weaponGo.AddComponent<MagicStaff>(enemyWeapon);
                    break;
            }

            return weaponGo;
        }
    }
}