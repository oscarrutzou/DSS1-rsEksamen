using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.Path;
using DoctorsDungeon.Factory;
using LiteDB;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DoctorsDungeon.LiteDB.NewDB
{
    public class DBSave
    {
        #region Start
        private static DBSave instance;

        public static DBSave Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBSave();
                    DataBasePath = GetStartPath();
                }
                return instance;
            }
        }

        public static string DataBasePath;

        private static string GetStartPath()
        {
            string baseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(baseDirectoryPath, $"data\\test");
            Directory.CreateDirectory(path);

            return Path.Combine(path, "LiteDB.db");
        }
        #endregion

        #region Grid
        public void SaveGrid(Grid grid)
        {
            if (grid == null) return;

            using var db = new LiteDatabase(DataBasePath);

            ILiteCollection<GridData> gridCollection = db.GetCollection<GridData>();

            GridData savedData = gridCollection.FindById(grid.Name);

            if (savedData == null)
            {
                SaveNewGrid(gridCollection, grid);
            }
            else
            {
                if (grid.Width != savedData.GridSize[0] || grid.Height != savedData.GridSize[1])
                {
                    gridCollection.Delete(grid.Name);
                    SaveNewGrid(gridCollection, grid);
                    return;
                }

                UpdateGrid(gridCollection, grid, savedData);
            }
        }

        /// <summary>
        /// To update the grids cells
        /// </summary>
        /// <param name="grid"></param>
        private void UpdateGrid(ILiteCollection<GridData> gridCollection, Grid grid, GridData oldSavedData)
        {
            List<CellData> newCells = new List<CellData>();

            foreach (CellData oldCellData in oldSavedData.Cells)
            {
                Point point = new(oldCellData.PointPosition[0], oldCellData.PointPosition[1]);

                GameObject cellGo = grid.Cells[point];

                Cell cell = cellGo.GetComponent<Cell>();

                CellData newCellData = new()
                {
                    Cell_ID = oldCellData.Cell_ID,
                    PointPosition = oldCellData.PointPosition,
                    Room_Nr = cell.RoomNr,
                    Cell_Type = cell.CellWalkableType,
                };

                newCells.Add(newCellData);
            }

            oldSavedData.Cells = newCells;

            gridCollection.Update(oldSavedData);
        }
        
        private GridData SaveNewGrid(ILiteCollection<GridData> gridCollection, Grid grid)
        {
            GridData newData = new()
            {
                Grid_Name = grid.Name,
                Position = new float[] { grid.StartPostion.X, grid.StartPostion.Y },
                GridSize = new int[] { grid.Width, grid.Height },
                Cells = GetNewCells(grid),
            };

            gridCollection.Insert(newData);

            return newData;
        }

        private List<CellData> GetNewCells(Grid grid)
        {
            List<CellData> cells = new List<CellData>();
            foreach (GameObject go in grid.Cells.Values)
            {
                Point gridpos = go.Transform.GridPosition;
                Cell cell = go.GetComponent<Cell>();

                // The cell data we want to save
                CellData cellData = new()
                {
                    Cell_ID = Guid.NewGuid(), // A unique key
                    PointPosition = new int[] { gridpos.X, gridpos.Y },
                    Room_Nr = cell.RoomNr,
                    Cell_Type = cell.CellWalkableType,
                };

                cells.Add(cellData);
            }
            return cells;
        }


        public GameObject LoadGrid(string gridname)
        {
            using var db = new LiteDatabase(DataBasePath);
            ILiteCollection<GridData> gridCollection = db.GetCollection<GridData>();
            
            GameObject gridGo = new();

            GridData savedGrid = gridCollection.FindById(gridname);

            if (savedGrid == null) return null;

            Grid grid = gridGo.AddComponent<Grid>(savedGrid.Grid_Name,
                                                  new Vector2(savedGrid.Position[0], savedGrid.Position[1]),
                                                  savedGrid.GridSize[0],
                                                  savedGrid.GridSize[1]);

            foreach (CellData cellData in savedGrid.Cells)
            {
                Point gridPos = new(cellData.PointPosition[0], cellData.PointPosition[1]);
                GameObject cellGo = CellFactory.Create(grid, gridPos, cellData.Cell_Type, cellData.Room_Nr); // Create the cell object
                grid.Cells.Add(gridPos, cellGo); // Adds to Cell Dict so we can use it later.
                cellGo.IsEnabled = InputHandler.Instance.DebugMode;
                GameWorld.Instance.Instantiate(cellGo); //Spawn it into the world.
            }

            return gridGo;
        }
        #endregion

    }

}