using DoctorsDungeon.ComponentPattern.Path;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.LiteDB.NewDB
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
}
