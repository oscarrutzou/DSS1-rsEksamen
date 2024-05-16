using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.ObserverPattern;
using FørsteÅrsEksamen.RepositoryPattern;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FørsteÅrsEksamen.ComponentPattern.Path
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
        public Grid SelectedGrid { get; private set; }

        private bool showGrid = true;

        private int roomNrIndex = 1;

        /// <summary>
        /// For the room index on the cells, right now there is a limit of 10 rooms.
        /// </summary>
        public int RoomNrIndex
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

        private float overrrideTime = 1; // How long there should go between grid saves.
        private float overrideUpdateTimer;

        private IRepository repository;

        private List<IObserver> gricCangeObservers = new();

        #endregion Parameters

        // Måske skal der ikke være en gridmanager siden der nok max vil være 1 grid på
        public GridManager()
        {
            repository = FileRepository.Instance; //Måske lav rep til en static som er den måde den save på, som bliver bestemt i starten.
                                                  // Lav det til at alt er saved på pc og hvis timestamp er anderledet på postgre end file, skal den først uploade alt hvis den har adgang, før den starter?
                                                  // Brug file system hvis der ikke er adgang til postgre
        }

        public void Update()
        {
            overrideUpdateTimer -= GameWorld.DeltaTime;

            if (overrideUpdateTimer < 0)
            {
                overrideUpdateTimer = overrrideTime;
                OverrideSaveGrid(); // Works since we're just changing the CurrentGrid in the GridManager
            }
        }
        public void ChangeRoomNrIndex(int addToCurrentRoomNr) => RoomNrIndex += addToCurrentRoomNr;

        #region SaveLoad
        public void SaveGrid(Grid grid)
        {
            CurrentGrid = grid;

            if (!repository.DoesGridExist(grid.Name))
            {
                OverrideSaveGrid();
            }
            else
            {
                LoadGrid(grid.Name);
            }
        }

        public void OverrideSaveGrid()
        {
            if (CurrentGrid == null) return;

            repository.SaveGrid(CurrentGrid);
        }

        public void LoadGrid(string gridName)
        {
            // A little dumb that it first gets made and then deleted? Fix, if u have time
            DeleteDrawnGrid();
            GameObject go = repository.GetGrid(gridName);
            CurrentGrid = go.GetComponent<Grid>();
            //ShouldDrawCells();
        }
        #endregion

        #region Draw and Remove Current Grid
        public void DrawOnCells()
        {
            if (GuiMethods.IsMouseOverUI()) return;

            GameObject cellGo = GetCellAtPos(InputHandler.Instance.MouseInWorld);
            if (cellGo == null) return;

            Cell cell = cellGo.GetComponent<Cell>();
            SetCellProperties(cell, CellWalkableType.FullValid, RoomNrIndex); // Move the WalkableType out of this room
        }

        public void SetDefaultOnCell()
        {
            if (GuiMethods.IsMouseOverUI()) return;

            GameObject cellGo = GetCellAtPos(InputHandler.Instance.MouseInWorld);
            if (cellGo == null) return;

            Cell cell = cellGo.GetComponent<Cell>();
            SetCellProperties(cell, CellWalkableType.NotValid, -1);
        }

        private void SetCellProperties(Cell cell, CellWalkableType walkableType, int roomNr)
        {
            if (!showGrid) return; // You cant draw on the grid when its not set

            cell.GameObject.GetComponent<SpriteRenderer>().ShouldDraw = true; // Need to be true, so its wlakabletype gets set proberly.
            cell.RoomNr = roomNr;
            cell.ChangeCellWalkalbeType(walkableType);

            // Add or remove fomr targetCells in current grid.
            // When loading new grid, add all the cells that have CellWalkableType = FullValid
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
        #endregion

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
            showGrid = !showGrid;

            ShouldDrawCells();
        }

        private void ShouldDrawCells()
        {
            if (CurrentGrid == null) return;

            foreach (GameObject go in CurrentGrid.Cells.Values)
            {
                // Set all to getting draw if the bool is true.
                go.GetComponent<SpriteRenderer>().ShouldDraw = showGrid;

                Cell cell = go.GetComponent<Cell>();
                cell.ChangeCellWalkalbeType(cell.CellWalkableType); // Only draw the ones that have a room.
            }
        } 

        #endregion

        #region Observer Pattern
        public void Attach(IObserver observer)
        {
            gricCangeObservers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            gricCangeObservers.Remove(observer);
        }

        public void Notify()
        {
            foreach (IObserver item in gricCangeObservers)
            {
                item.UpdateObserver();
            }
        }
        #endregion
    }
}