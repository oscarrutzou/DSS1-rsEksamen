using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.Factory;
using FørsteÅrsEksamen.GameManagement;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.RepositoryPattern
{
    public static class DBGrid
    {
        public static bool DoesGridExits(string name)
        {
            using var db = new DataBase(CollectionName.Grids);
            var gridExits = db.FindOne<GridData>(x => x.Grid_Name == name);
            return gridExits != null;
        }

        /// <summary>
        /// Saves a Grid into LiteDB
        /// </summary>
        /// <param name="grid"></param>
        public static void SaveGrid(Grid grid)
        {
            if (grid == null) return;

            // Open a new Database with the Grids connection.
            using var gridDB = new DataBase(CollectionName.Grids);

            // The grid data we want to store
            GridData gridData = new()
            {
                Grid_Name = grid.Name,
                PositionX = grid.StartPostion.X,
                PositionY = grid.StartPostion.Y,
                Start_Width = grid.Width,
                Start_Height = grid.Height,
            };

            // Checks if there already is a grid
            var gridCollection = gridDB.GetCollection<GridData>();

            var gridExits = gridCollection.FindOne(x => x.Grid_Name == gridData.Grid_Name);

            // If there is a grid, we delete the grid and all connections to the grid.
            if (gridExits != null)
            {
                //Delete current grid and its cells.
                DeletesCells(gridData);
                // Delete the grid
                gridDB.Delete<GridData>(new BsonValue(gridData.Grid_Name));
            }

            // We save the new grid
            gridDB.SaveSingle(gridData, x => x.Grid_Name == gridData.Grid_Name);

            // Open two new Database
            using var cellsDB = new DataBase(CollectionName.Cells);
            using var gridHasCellsDB = new DataBase(CollectionName.GridHasCells);

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
                        PointPositionX = gridpos.X,
                        PointPositionY = gridpos.Y,
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
            using var cellsDB = new DataBase(CollectionName.Cells);
            using var gridHasCellsDB = new DataBase(CollectionName.GridHasCells);

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
            using var gridDB = new DataBase(CollectionName.Grids);
            using var cellsDB = new DataBase(CollectionName.Cells);
            using var gridHasCellsDB = new DataBase(CollectionName.GridHasCells);

            GameObject gridGo = new();

            // Finds if there is a grid in the GridData collection.
            GridData gridData = gridDB.FindOne<GridData>(x => x.Grid_Name == gridName);

            if (gridData == null) return null; //No Grid found

            // Add the component with the correct data we found before.
            Grid grid = gridGo.AddComponent<Grid>(gridData.Grid_Name,
                                                  new Vector2(gridData.PositionX, gridData.PositionY),
                                                  gridData.Start_Width,
                                                  gridData.Start_Height);

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
                Point gridPos = new Point(cellData.PointPositionX, cellData.PointPositionY); // Grid position
                GameObject cellGo = CellFactory.Create(grid, gridPos, cellData.Cell_Type, cellData.Room_Nr); // Create the cell object
                grid.Cells.Add(gridPos, cellGo); // Adds to Cell Dict so we can use it later.
                GameWorld.Instance.Instantiate(cellGo); //Spawn it into the world.
            }

            return gridGo;
        }


    }
}
