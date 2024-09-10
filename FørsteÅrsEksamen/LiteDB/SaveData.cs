using ShamansDungeon.ComponentPattern.PlayerClasses;
using ShamansDungeon.Factory;
using System.Collections.Generic;

namespace ShamansDungeon.LiteDB;

// Oscar
public static class SaveData
{
    private const int _startCurrency = 100; // Normal 100
    private static double _startTimeLeft = 180f; //

    private const int _cheatMultiplier = 100;

    public static int CurrentSaveID = 1; //Gets set by player and determins loaded data
    public const int MaxSaveID = 3;
    public static int Currency;
    public static bool HasCompletedFullTutorial;
    public static int TutorialReached;
    // Save what classes and weapons are unlocked
    public static List<WeaponTypes> UnlockedWeapons { get; set; } = new();

    public static List<ClassTypes> UnlockedClasses { get; set; } = new();

    public static int Level_Reached { get; set; } = 1;
    public const int MaxRooms = 4;
    public static double Time_Left { get; set; } = _startTimeLeft;
    public static bool HasWon { get; set; }
    public static bool LostByTime;

    public static WeaponTypes SelectedWeapon { get; set; }
    public static ClassTypes SelectedClass { get; set; }
    public static Player Player { get; set; }

    public static void SetBaseValues()
    {
        Currency = _startCurrency;
        Time_Left = _startTimeLeft;
        if (GameWorld.DebugAndCheats)
        {
            Currency *= _cheatMultiplier;
            Time_Left *= _cheatMultiplier;
        }
        Level_Reached = 1;
        UnlockedWeapons = new();
        UnlockedClasses = new();
        Player = null;
        HasCompletedFullTutorial = false;
        TutorialReached = 0;
    }

    public static void ResetPlayer()
    {
        Player = null;
    }
}