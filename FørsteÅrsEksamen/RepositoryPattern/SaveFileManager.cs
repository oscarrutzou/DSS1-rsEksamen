using FørsteÅrsEksamen.ComponentPattern.Classes;
using FørsteÅrsEksamen.Factory;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    public static class SaveFileManager
    {
        public static int CurrentSaveID = 1; //Gets set by player and determins loaded data
        public static int MaxSaveID = 3;
        public static int Currency = 0;

        // Save what classes and weapons are unlocked
        public static List<WeaponTypes> UnlockedWeapons = new() { WeaponTypes.Sword, WeaponTypes.Axe, };

        public static List<ClassTypes> UnlockedClasses = new() { ClassTypes.Warrior, ClassTypes.Archer };

        public static int Room_Reached = 1;
        public static float Time_Left = 300f;

        public static Player Player;
    }
}