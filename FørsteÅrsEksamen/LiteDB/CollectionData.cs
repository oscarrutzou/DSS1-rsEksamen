using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.Factory;
using LiteDB;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.LiteDB;

// Oscar
public class GridData
{
    [BsonId]
    public string Grid_Name { get; set; }

    public float[] Position { get; set; }
    public int[] GridSize { get; set; }
    public List<CellData> Cells { get; set; }
}

public class CellData
{
    [BsonId]
    public Guid Cell_ID { get; set; }

    public int[] PointPosition { get; set; }
    public int CollisionNr { get; set; }
    public int RoomNr { get; set; }

    public CellWalkableType Cell_Type { get; set; }
}

public class SaveFileData
{
    [BsonId]
    public int Save_ID { get; set; }

    public int Currency { get; set; }
    public DateTime Last_Login { get; set; }
    public List<ClassTypes> Unlocked_Classes { get; set; }
    public List<WeaponTypes> Unlocked_Weapons { get; set; }
    public RunData RunData { get; set; }
    public bool HasCompletedFullTutorial { get; set; }
    public SaveFileData()
    {
        Last_Login = DateTime.Now;
    }
}

public class RunData
{
    public int Room_Reached { get; set; }
    public double Time_Left { get; set; }
    public PlayerData PlayerData { get; set; }
}

public class PlayerData
{
    public int Health { get; set; }
    public string Potion_Name { get; set; }
    public ClassTypes Class_Type { get; set; }
    public WeaponTypes Weapon_Type { get; set; }
}