using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.ObserverPattern;
using FørsteÅrsEksamen.RepositoryPattern;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.ComponentPattern.Path
{
    public class GridManager : ISubject
    {
        #region Parameters

        private static GridManager instance;

        public static GridManager Instance
        { get { return instance ??= instance = new GridManager(); } }

        public Grid CurrentGrid;
        public Grid SelectedGrid { get; private set; }

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

        private IRepository repository;

        #endregion Parameters

        // Måske skal der ikke være en gridmanager siden der nok max vil være 1 grid på
        public GridManager()
        {
            repository = FileRepository.Instance; //Måske lav rep til en static som er den måde den save på, som bliver bestemt i starten.
                                                  // Lav det til at alt er saved på pc og hvis timestamp er anderledet på postgre end file, skal den først uploade alt hvis den har adgang, før den starter?
                                                  // Brug file system hvis der ikke er adgang til postgre
        }

        private float overrrideTime = 1;
        private float overrideUpdateTimer;

        public void Update()
        {
            overrideUpdateTimer -= GameWorld.DeltaTime;

            if (overrideUpdateTimer < 0)
            {
                overrideUpdateTimer = overrrideTime;
                OverrideSaveGrid(); // Works since we're just changing the CurrentGrid in the GridManager
            }
        }

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
        }

        public void DrawOnCells()
        {
            if (GuiMethods.IsMouseOverUI()) return;

            GameObject cellGo = GetCellAtPos(InputHandler.Instance.mouseInWorld);
            if (cellGo == null) return;

            Cell cell = cellGo.GetComponent<Cell>();
            SetCellProperties(cell, CellWalkableType.FullValid, RoomNrIndex); // Move the WalkableType out of this room
        }

        public void SetDefaultOnCell()
        {
            if (GuiMethods.IsMouseOverUI()) return;

            GameObject cellGo = GetCellAtPos(InputHandler.Instance.mouseInWorld);
            if (cellGo == null) return;

            Cell cell = cellGo.GetComponent<Cell>();
            SetCellProperties(cell, CellWalkableType.NotValid, -1);
        }

        private void SetCellProperties(Cell cell, CellWalkableType walkableType, int roomNr)
        {
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

        //public Point GetPointAtPos(Vector2 pos) => GetCellAtPos(pos).Transform.GridPosition;

        public void ChangeRoomNrIndex(int addToCurrentRoomNr) => RoomNrIndex += addToCurrentRoomNr;

        public void Attach(IObserver observer)
        {
        }

        public void Detach(IObserver observer)
        {
        }

        public void Notify()
        {
        }
    }
}