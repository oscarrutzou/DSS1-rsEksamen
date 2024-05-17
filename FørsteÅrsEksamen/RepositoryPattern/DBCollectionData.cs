using FørsteÅrsEksamen.ComponentPattern.Characters;
using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.Factory;
using LiteDB;
using System;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    public class SaveFile
    {
        [BsonId]
        public Guid Save_ID { get; set; }
        public int Currency { get; set; }
        public DateTime Last_Login { get; set; }
        public SaveFile()
        {
            Last_Login = DateTime.Now;
        }
    }

    public class UnlockedClass
    {
        [BsonId]
        public ClassTypes Class_Type { get; set; }
    }
    public class SaveFileHasUnlockedClass
    {
        public Guid Save_ID { get; set; }
        public ClassTypes Class_Type { get; set; }
    }
    public class UnclockedWeapon
    {
        [BsonId]
        public WeaponTypes Weapon_Type { get; set; }
    }
    public class SaveFileHasUnclockedWeapon
    {
        public Guid Save_ID { get; set; }
        public WeaponTypes Weapon_Type { get; set; }
    }

    public class SaveFileHasRunData
    {
        [BsonId]
        public Guid Save_ID { get; set; }
        public Guid Run_ID { get; set; }
    }

    public class RunData
    {
        [BsonId]
        public Guid Run_ID { get; set; }
        public int Room_Reached { get; set; }
        public float Time_Left { get; set; }
    }

    public class RunDataHasPlayerData
    {
        [BsonId]
        public Guid Run_ID { get; set; }
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
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public int Start_Width { get; set; }
        public int Start_Height { get; set; }
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
        public int PointPositionX { get; set; }
        public int PointPositionY { get; set; }
        public int Room_Nr { get; set; }
        public CellWalkableType Cell_Type { get; set; }
    }
}
