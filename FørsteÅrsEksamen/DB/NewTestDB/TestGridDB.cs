using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Path;
using LiteDB;
using System.IO;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace FørsteÅrsEksamen.DB.NewTestDB
{
    public static class TestGridDB
    {


        public static void SaveGrid(Grid grid)
        {
            if (grid == null) return;

            List<CellTestData> cells = new();

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    // Makes some variables to make code more manageable
                    GameObject cellGo = grid.Cells[new Point(x, y)];
                    Point gridpos = cellGo.Transform.GridPosition;
                    Cell cell = cellGo.GetComponent<Cell>();

                    CellTestData cellData = new()
                    {
                        Cell_ID = Guid.NewGuid(), // A unique key
                        PointPosition = new int[] { gridpos.X, gridpos.Y },
                        Room_Nr = cell.RoomNr,
                        Cell_Type = cell.CellWalkableType,
                    };

                    cells.Add(cellData);
                }
            }

            GridTestData gridData = new GridTestData()
            {
                Grid_Name = grid.Name + "Unga",
                GridSize = new int[] { grid.Width, grid.Height },
                Position = new float[] { grid.StartPostion.X, grid.StartPostion.Y },
                Cells = cells,
            };

            using var gridDB = new DataBase(CollectionName.GridTest, "grids");

            gridDB.SaveSingle(gridData);
        }

        //public GameObject LoadGrid(string gridName)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool DoesGridExits(string gridName)
        //{
        //    throw new NotImplementedException();
        //}


    }

}
