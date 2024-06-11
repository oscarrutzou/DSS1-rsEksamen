using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.Factory;
using System.Collections.Generic;

namespace DoctorsDungeon.LiteDB
{
    // Oscar
    public static class SaveData
    {
        private const int StartCurrency = 100;
        private static float StartTimeLeft = 120f;

        public static int CurrentSaveID = 1; //Gets set by player and determins loaded data
        public const int MaxSaveID = 3;
        public static int Currency = 100; // Start currency is 100

        // Save what classes and weapons are unlocked
        public static List<WeaponTypes> UnlockedWeapons { get; set; } = new();

        public static List<ClassTypes> UnlockedClasses { get; set; } = new();

        public static int Level_Reached { get; set; } = 1;
        public const int MaxRooms = 3;
        public static float Time_Left { get; set; } = StartTimeLeft;
        public static bool HasWon { get; set; }
        public static bool LostByTime;

        public static WeaponTypes SelectedWeapon { get; set; }
        public static ClassTypes SelectedClass { get; set; }
        public static Player Player { get; set; }

        public static void SetBaseValues()
        {
            Currency = StartCurrency;
            Time_Left = StartTimeLeft;
            Level_Reached = 1;
            UnlockedWeapons = new();
            UnlockedClasses = new();
            Player = null;
        }
    }
}