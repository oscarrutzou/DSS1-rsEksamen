﻿using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.LiteDB;
using DoctorsDungeon.ObserverPattern;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Path
{
    public class GridManager : ISubject
    {
        #region Parameters

        private static GridManager instance;

        public static GridManager Instance
        { get { return instance ??= instance = new GridManager(); } }

        private Grid currentGrid;

        public Grid CurrentGrid
        {
            get { return currentGrid; }
            private set
            {
                if (value != currentGrid)
                {
                    currentGrid = value;
                    Notify();
                }
            }
        }

        public bool DrawRoomNr { get; set; }

        private int roomNrIndex = 1;

        /// <summary>
        /// For the room index on the cells, right now there is a limit of 10 rooms.
        /// </summary>
        public int LevelNrIndex
        {
            get { return roomNrIndex; }
            set
            {
                if (value == 0 || value > 10) return;

                if (roomNrIndex != value)
                {
                    roomNrIndex = value;
                }
            }
        }

        private List<IObserver> gridChangeObservers = new();

        #endregion Parameters

        public void ChangeRoomNrIndex(int addToCurrentRoomNr)
        {
            if (!InputHandler.Instance.DebugMode) return;
            LevelNrIndex += addToCurrentRoomNr;
        }

        #region SaveLoad

        public void SaveLoadGrid(Grid grid)
        {
            CurrentGrid = grid;

            if (DBGrid.DoesGridExits(CurrentGrid.Name))
            {
                // Load grid
                LoadGrid(CurrentGrid.Name);
            }
            else
            {
                // Save Grid
                DBGrid.OverrideSaveGrid(CurrentGrid);

                // Draw Grid
                foreach (GameObject cellGo in CurrentGrid.Cells.Values)
                {
                    GameWorld.Instance.Instantiate(cellGo);
                }
            }
        }

        // A bug in the update grid cells
        //public void UpdateGrid(Grid grid)
        //{
        //    if (DBGrid.DoesGridExits(grid.Name))
        //    {
        //        DBGrid.UpdateGridCells(grid);
        //    }
        //    else
        //    {
        //        // Save Grid
        //        DBGrid.OverrideSaveGrid(CurrentGrid);

        //        // Draw Grid
        //        foreach (GameObject cellGo in CurrentGrid.Cells.Values)
        //        {
        //            GameWorld.Instance.Instantiate(cellGo);
        //        }
        //    }
        //}

        public void LoadGrid(string gridName)
        {
            GameObject go = DBGrid.GetGrid(gridName);

            if (go == null)
            {
                CurrentGrid = null;
                return; //Didnt find the Grid in the repository.
            }

            CurrentGrid = go.GetComponent<Grid>();
        }

        #endregion SaveLoad

        #region Draw and Remove Current Grid

        public void DrawOnCells()
        {
            if (!InputHandler.Instance.DebugMode
                || GuiMethods.IsMouseOverUI()) return;

            GameObject cellGo = GetCellAtPos(InputHandler.Instance.MouseInWorld);
            if (cellGo == null) return;

            SetCellProperties(cellGo, CellWalkableType.FullValid, LevelNrIndex); // Move the is walkable out of this
        }

        public void SetDefaultOnCell()
        {
            if (!InputHandler.Instance.DebugMode
                || GuiMethods.IsMouseOverUI()) return;

            GameObject cellGo = GetCellAtPos(InputHandler.Instance.MouseInWorld);
            if (cellGo == null) return;

            SetCellProperties(cellGo, CellWalkableType.NotValid, -1);
        }

        private void SetCellProperties(GameObject cellGo, CellWalkableType walkableType, int roomNr)
        {
            Cell cell = cellGo.GetComponent<Cell>();
            cell.RoomNr = roomNr;
            cell.ChangeCellWalkalbeType(walkableType);
        }

        public void DeleteDrawnGrid()
        {
            if (CurrentGrid == null) return;

            foreach (GameObject cellGo in CurrentGrid.Cells.Values)
            {
                GameWorld.Instance.Destroy(cellGo);
            }
            GameWorld.Instance.Destroy(CurrentGrid.GameObject);
            CurrentGrid = null;
        }

        #endregion Draw and Remove Current Grid

        #region Return Methods

        public GameObject GetCellAtPos(Vector2 pos)
        {
            if (CurrentGrid == null) return null;

            GameObject go = CurrentGrid.GetCellGameObject(pos);
            if (go != null)
            {
                return go;
            }

            return null;
        }

        public Vector2 GetCornerPositionOfCell(Point gridCell)
        {
            Vector2 temp = Vector2.Zero;
            if (CurrentGrid == null) return temp;

            temp = CurrentGrid.PosFromGridPos(gridCell);
            temp -= new Vector2(Cell.dimension * Cell.Scale / 2, Cell.dimension * Cell.Scale / 2);

            return temp;
        }

        public void ShowHideGrid()
        {
            InputHandler.Instance.DebugMode = !InputHandler.Instance.DebugMode;

            if (!InputHandler.Instance.DebugMode) return;

            ShouldDrawCells();
        }

        private void ShouldDrawCells()
        {
            if (CurrentGrid == null) return;

            foreach (GameObject go in CurrentGrid.Cells.Values)
            {
                // Set all to getting draw if the bool is true.
                //go.GetComponent<SpriteRenderer>().ShouldDraw = InputHandler.Instance.DebugMode;
                go.IsEnabled = InputHandler.Instance.DebugMode;
                Cell cell = go.GetComponent<Cell>();
                cell.ChangeCellWalkalbeType(cell.CellWalkableType); // Only draw the ones that have a room.
            }
        }

        #endregion Return Methods

        #region Observer Pattern

        public void Attach(IObserver observer)
        {
            gridChangeObservers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            gridChangeObservers.Remove(observer);
        }

        public void Notify()
        {
            foreach (IObserver item in gridChangeObservers)
            {
                item.UpdateObserver();
            }
        }

        #endregion Observer Pattern
    }
}