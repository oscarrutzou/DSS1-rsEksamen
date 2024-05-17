using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.ComponentPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.Internal;
using Microsoft.Xna.Framework;
using LiteDB;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    public static class DBMethods
    {

        #region Grids
        public static void SaveGrid(Grid grid)
        {
            if (grid == null) return;

            // Make a grid db
            using var gridDB = new DataBase(CollectionName.Grids);

            GridData gridData = new GridData()
            {
                Grid_Name = grid.Name,
                PositionX = grid.StartPostion.X,
                PositionY = grid.StartPostion.Y,
                Start_Width = grid.Width,
                Start_Height = grid.Height,
            };

            var gridCollection = gridDB.GetCollection<GridData>(CollectionName.Grids);

            var gridExits = gridCollection.FindOne(x => x.Grid_Name == gridData.Grid_Name);

            if (gridExits != null)
            {
                //Delete current grid and its cells.
                DeleteGridAndCells(gridData);
                // Delete the grid
                gridDB.Delete<GridData>(new BsonValue(gridData.Grid_Name));
            }

            gridDB.SaveSingle(gridData, x => x.Grid_Name == gridData.Grid_Name);


            using var cellsDB = new DataBase(CollectionName.Cells);
            using var gridHasCellsDB = new DataBase(CollectionName.GridHasCells);

            // Make cells
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    GameObject cellGo = grid.Cells[new Point(x, y)];
                    Point gridpos = cellGo.Transform.GridPosition;
                    Cell cell = cellGo.GetComponent<Cell>();

                    CellData cellData = new CellData()
                    {
                        Cell_ID = Guid.NewGuid(),
                        PointPositionX = gridpos.X,
                        PointPositionY = gridpos.Y,
                        Room_Nr = cell.RoomNr,
                        Cell_Type = cell.CellWalkableType,
                    };

                    cellsDB.SaveSingle(cellData);
                    cellsDB.EnsureIndex<CellData>(x => x.PointPositionX);
                    cellsDB.EnsureIndex<CellData>(x => x.PointPositionY);

                    GridHasCells gridHasCell = new GridHasCells()
                    {
                        Grid_Name = gridData.Grid_Name,
                        Cell_ID = cellData.Cell_ID,
                    };

                    gridHasCellsDB.SaveSingle(gridHasCell);
                }
            }
        }

        private static void DeleteGridAndCells(GridData gridData)
        {
            using var cellsDB = new DataBase(CollectionName.Cells);
            using var gridHasCellsDB = new DataBase(CollectionName.GridHasCells);

            // Get all the GridHasCells where the Grid_Name is the same as the Grid_Name in gridData
            var gridHasCells = gridHasCellsDB.GetCollection<GridHasCells>(CollectionName.GridHasCells)
                                             .Find(x => x.Grid_Name == gridData.Grid_Name)
                                             .ToList();

            // Delete every cell that has the same Cell_ID as the Cell_ID in gridHasCells
            foreach (var gridHasCell in gridHasCells)
            {
                cellsDB.Delete<CellData>(gridHasCell.Cell_ID);
            }

            // Delete everyone in GridHasCells that has the same Cell_ID as gridHasCell
            foreach (var gridHasCell in gridHasCells)
            {
                gridHasCellsDB.Delete<GridHasCells>(gridHasCell.Cell_ID);
            }
        }

        public static GameObject GetGrid(string description)
        {
            using var gridDB = new DataBase(CollectionName.Grids);
            using var cellsDB = new DataBase(CollectionName.Cells);
            using var gridHasCellsDB = new DataBase(CollectionName.GridHasCells);

            GameObject gridGo = new();

            GridData gridData = gridDB.FindOne<GridData>(x => x.Grid_Name == description);

            if (gridData == null) return null; //No Grid found

            Grid grid = gridGo.AddComponent<Grid>(gridData.Grid_Name, new Vector2(gridData.PositionX, gridData.PositionY), gridData.Start_Width, gridData.Start_Height);

            List<GridHasCells> gridHasCells = gridHasCellsDB.GetAll<GridHasCells>()
                                                            .Where(cell => cell.Grid_Name == gridData.Grid_Name)
                                                            .ToList();

            List<CellData> cellDataList = cellsDB.GetAll<CellData>()
                                                 .Where(cellData => gridHasCells.Any(gridHasCell => gridHasCell.Cell_ID == cellData.Cell_ID))
                                                 .ToList();

            foreach (CellData cellData in cellDataList)
            {
                Point gridPos = new Point(cellData.PointPositionX, cellData.PointPositionY);
                GameObject cellGo = CellFactory.Create(grid, gridPos, cellData.Cell_Type, cellData.Room_Nr);
                grid.Cells.Add(gridPos, cellGo);
                GameWorld.Instance.Instantiate(cellGo);
            }

            return gridGo;
        }
        #endregion
    }
}
