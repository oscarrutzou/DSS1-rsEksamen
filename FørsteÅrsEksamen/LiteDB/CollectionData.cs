using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.Factory;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.LiteDB
{
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
        public int Room_Nr { get; set; }
        public CellWalkableType Cell_Type { get; set; }
    }

    public class SaveFileTestData
    {
        [BsonId]
        public int Save_ID { get; set; }

        public int Currency { get; set; }
        public DateTime Last_Login { get; set; }
        public List<ClassTypes> Unlocked_Classes { get; set; }
        public List<WeaponTypes> Unlocked_Weapons { get; set; }
        public RunTestData RunData { get; set; }
        // Could just sate the rundata id
        public SaveFileTestData()
        {
            Last_Login = DateTime.Now;
        }
    }

    public class RunTestData
    {
        public int Room_Reached { get; set; }
        public float Time_Left { get; set; }
        public PlayerTestData PlayerData { get; set; }
    }

    public class PlayerTestData
    {
        public int Health { get; set; }
        public string Potion_Name { get; set; }
        public ClassTypes Class_Type { get; set; }
        public WeaponTypes Weapon_Type { get; set; }
    }
}
