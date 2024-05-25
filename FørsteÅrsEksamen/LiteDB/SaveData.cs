using FørsteÅrsEksamen.ComponentPattern.PlayerClasses;
using FørsteÅrsEksamen.Factory;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.LiteDB
{
    public static class SaveData
    {
        public static int CurrentSaveID = 1; //Gets set by player and determins loaded data
        public static int MaxSaveID = 3;
        public static int Currency = 0;

        // Save what classes and weapons are unlocked
        public static List<WeaponTypes> UnlockedWeapons = new() { WeaponTypes.Sword, WeaponTypes.Axe, };

        public static List<ClassTypes> UnlockedClasses = new() { ClassTypes.Warrior, ClassTypes.Archer };

        public static int Room_Reached { get; set; } = 1;
        public static int MaxRooms = 2;
        public static float Time_Left { get; set; } = 20f;
        public static bool HasWon { get; set; }
        public static bool LostByTime;
       
        public static WeaponTypes SelectedWeapon; 
        public static ClassTypes SelectedClass {  get; set; }
        public static Player Player { get; set; }

        public static void SetBaseValues()
        {
            Currency = 0;
            Time_Left = 20f;
            Room_Reached = 1;
            UnlockedWeapons = new();
            UnlockedClasses = new();
            Player = null;
        }
    }
}