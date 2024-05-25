using FørsteÅrsEksamen.ComponentPattern.PlayerClasses;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.Factory;
using LiteDB;
using System;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.LiteDB
{

    public class GridTestData
    {
        [BsonId]
        public string Grid_Name { get; set; }

        public float[] Position { get; set; }
        public int[] GridSize { get; set; }
        public List<CellTestData> Cells { get; set; }
    }

    public class CellTestData
    {
        [BsonId]
        public Guid Cell_ID { get; set; }
        public int[] PointPosition { get; set; }
        public int Room_Nr { get; set; }
        public CellWalkableType Cell_Type { get; set; }
    }

    //}

    //public class GridHasCells
    //{
    //    public string Grid_Name { get; set; }

    //    [BsonId]
    //    public Guid Cell_ID { get; set; }
    //}

    //public class CellData
    //{


    public class SaveFileData
    {
        [BsonId]
        public int Save_ID { get; set; }
        public int Currency { get; set; }
        public DateTime Last_Login { get; set; }

        public SaveFileData()
        {
            Last_Login = DateTime.Now;
        }
    }

    public class UnlockedClassData
    {
        [BsonId]
        public Guid Class_ID { get; set; }
        public ClassTypes Class_Type { get; set; }
    }

    public class SaveFileHasUnlockedClass
    {
        [BsonId]
        public Guid Class_ID { get; set; }
        public int Save_ID { get; set; }
    }

    public class UnlockedWeaponData
    {
        [BsonId]
        public Guid Weapon_ID { get; set; }
        public WeaponTypes Weapon_Type { get; set; }
    }

    public class SaveFileHasUnlockedWeapon
    {
        [BsonId]
        public Guid Weapon_ID { get; set; }
        public int Save_ID { get; set; }
    }

    public class SaveFileHasRunData
    {
        [BsonId]
        public int Save_ID { get; set; }
        public int Run_ID { get; set; }
    }

    public class RunData
    {
        [BsonId]
        public int Run_ID { get; set; }
        public int Room_Reached { get; set; }
        public float Time_Left { get; set; }
    }

    public class RunDataHasPlayerData
    {
        [BsonId]
        public int Run_ID { get; set; }
        public Guid Player_ID { get; set; }
    }

    public class PlayerData
    {
        [BsonId]
        public Guid Player_ID { get; set; }

        public int Health { get; set; }
        public string Potion_Name { get; set; }
        public ClassTypes Class_Type { get; set; }
        public WeaponTypes Weapon_Type { get; set; }
    }

    public class GridData
    {
        [BsonId]
        public string Grid_Name { get; set; }
        public float[] Position { get; set; }
        public int[] Start_Size { get; set; }
    }

    public class GridHasCells
    {
        public string Grid_Name { get; set; }

        [BsonId]
        public Guid Cell_ID { get; set; }
    }

    public class CellData
    {
        [BsonId]
        public Guid Cell_ID { get; set; }

        public int[] PointPosition { get; set; }
        public int Room_Nr { get; set; }
        public CellWalkableType Cell_Type { get; set; }
    }
}