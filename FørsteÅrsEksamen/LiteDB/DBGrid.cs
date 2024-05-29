using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.Factory;
using LiteDB;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoctorsDungeon.LiteDB
{
    // Oscar
    public static class DBGrid
    {
        private const string gridFolder = "grids";

        public static bool DoesGridExits(string name)
        {
            using var db = new DataBase(CollectionName.Grids, gridFolder);
            var gridExits = db.FindOne<GridData>(x => x.Grid_Name == name);
            return gridExits != null;
        }

        public static void DeleteGrid(string name)
        {
            using var gridDB = new DataBase(CollectionName.Grids, gridFolder);
            GridData data = gridDB.FindOne<GridData>(x => x.Grid_Name == name);

            if (data == null) return; // No grid to delete

            DeletesCells(data); // Deltes cell and the connection collection
            gridDB.Delete<GridData>(new BsonValue(data.Grid_Name)); // Deletes the grid
        }

        /// <summary>
        /// Updates the RoomNr and CellType on every saved cell.
        /// </summary>
        /// <param name="grid"></param>
        //public static void UpdateGridCells(Grid grid)
        //{
        //    using var cellsDB = new DataBase(CollectionName.Cells, gridFolder);
        //    var cellsCollection = cellsDB.GetCollection<CellData>();

        //    foreach (GameObject cellGo in grid.Cells.Values)
        //    {
        //        Cell cell = cellGo.GetComponent<Cell>();
        //        int[] PointPosition = new int[] { cellGo.Transform.GridPosition.X, cellGo.Transform.GridPosition.Y };

        //        CellData currentSavedData = cellsCollection.FindOne(x => x.PointPosition == PointPosition) ?? throw new Exception($"Cant find the cell for grid position {PointPosition}");

        //        CellData newCellData = new()
        //        {
        //            Cell_ID = currentSavedData.Cell_ID,
        //            PointPosition = PointPosition,
        //            Room_Nr = cell.RoomNr,
        //            Cell_Type = cell.CellWalkableType,
        //        };
        //        //cellsCollection.UpdateMany // Could use this? Maybe rework it
        //        cellsDB.UpdateReplaceData(currentSavedData.Cell_ID, newCellData);
        //    }
        //}

        /// <summary>
        /// Saves a Grid into LiteDB
        /// </summary>
        /// <param name="grid"></param>
        public static void OverrideSaveGrid(Grid grid)
        {
            if (grid == null) return;

            // The grid data we want to store
            GridData gridData = new()
            {
                Grid_Name = grid.Name,
                Position = new float[] { grid.StartPostion.X, grid.StartPostion.Y },
                Start_Size = new int[] { grid.Width, grid.Height },
            };

            DeleteGrid(grid.Name);

            // Open a new Database with the Grids connection.
            using var gridDB = new DataBase(CollectionName.Grids, gridFolder);

            // Checks if there already is a grid
            var gridCollection = gridDB.GetCollection<GridData>();

            // We save the new grid
            gridDB.SaveSingle(gridData, x => x.Grid_Name == gridData.Grid_Name);

            // Open two new Database
            using var cellsDB = new DataBase(CollectionName.Cells, gridFolder);
            using var gridHasCellsDB = new DataBase(CollectionName.GridHasCells, gridFolder);

            // Saves cells
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    // Makes some variables to make code more manageable
                    GameObject cellGo = grid.Cells[new Point(x, y)];
                    Point gridpos = cellGo.Transform.GridPosition;
                    Cell cell = cellGo.GetComponent<Cell>();

                    // The cell data we want to save
                    CellData cellData = new()
                    {
                        Cell_ID = Guid.NewGuid(), // A unique key
                        PointPosition = new int[] { gridpos.X, gridpos.Y },
                        Room_Nr = cell.RoomNr,
                        Cell_Type = cell.CellWalkableType,
                    };

                    // Saves cell
                    cellsDB.SaveSingle(cellData);

                    // The connection between the grid and each cell
                    GridHasCells gridHasCell = new()
                    {
                        Grid_Name = gridData.Grid_Name,
                        Cell_ID = cellData.Cell_ID,
                    };

                    // Save the connection
                    gridHasCellsDB.SaveSingle(gridHasCell);
                }
            }
        }

        /// <summary>
        /// Deletes the Cells and GridHasCells collections.
        /// </summary>
        /// <param name="gridData"></param>
        private static void DeletesCells(GridData gridData)
        {
            // Makes two connections to each collection.
            using var cellsDB = new DataBase(CollectionName.Cells, gridFolder);
            using var gridHasCellsDB = new DataBase(CollectionName.GridHasCells, gridFolder);

            // Get all the GridHasCells where the Grid_Name is the same as the Grid_Name in gridData
            var gridHasCells = gridHasCellsDB.GetCollection<GridHasCells>()
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

        /// <summary>
        /// Returns the Grid GameObject. The cells have been Instantiated
        /// </summary>
        /// <param name="gridName"></param>
        /// <returns>Grid GameObject</returns>
        public static GameObject GetGrid(string gridName)
        {
            using var gridDB = new DataBase(CollectionName.Grids, gridFolder);
            using var cellsDB = new DataBase(CollectionName.Cells, gridFolder);
            using var gridHasCellsDB = new DataBase(CollectionName.GridHasCells, gridFolder);

            GameObject gridGo = new();

            // Finds if there is a grid in the GridData collection.
            GridData gridData = gridDB.FindOne<GridData>(x => x.Grid_Name == gridName);

            if (gridData == null) return null; //No Grid found

            // Add the component with the correct data we found before.
            Grid grid = gridGo.AddComponent<Grid>(gridData.Grid_Name,
                                                  new Vector2(gridData.Position[0], gridData.Position[1]),
                                                  gridData.Start_Size[0],
                                                  gridData.Start_Size[1]);

            // Find each GridHasCells datatype in the GridHasCells collection. Only if the Grid Name is the same as the grid we want to load.
            List<GridHasCells> gridHasCells = gridHasCellsDB.GetAll<GridHasCells>()
                                                            .Where(cell => cell.Grid_Name == gridData.Grid_Name)
                                                            .ToList();

            // Get all CellData that has the same ID as the ID we found before.
            List<CellData> cellDataList = cellsDB.GetAll<CellData>()
                                                 .Where(cellData => gridHasCells.Any(gridHasCell => gridHasCell.Cell_ID == cellData.Cell_ID))
                                                 .ToList();

            // Go though the cell data and add it to the world and grid object.
            foreach (CellData cellData in cellDataList)
            {
                Point gridPos = new Point(cellData.PointPosition[0], cellData.PointPosition[1]); // Grid position
                GameObject cellGo = CellFactory.Create(grid, gridPos, cellData.Cell_Type, cellData.Room_Nr); // Create the cell object
                grid.Cells.Add(gridPos, cellGo); // Adds to Cell Dict so we can use it later.
                cellGo.IsEnabled = InputHandler.Instance.DebugMode;
                GameWorld.Instance.Instantiate(cellGo); //Spawn it into the world.
            }

            return gridGo;
        }
    }
}