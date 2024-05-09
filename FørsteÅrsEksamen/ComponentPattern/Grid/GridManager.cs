using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.RepositoryPattern;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.ComponentPattern.Grid
{
    public class GridManager
    {
        #region Parameters

        private static GridManager instance;

        public static GridManager Instance
        { get { return instance ??= instance = new GridManager(); } }

        public Grid CurrentGrid;
        public Grid SelectedGrid { get; private set; }

        private int gridIndex;

        public int GridIndex
        {
            get { return gridIndex; }
            set
            {
                if (gridIndex != value)
                {
                    gridIndex = value;
                    OnGridIndexChanged();
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
            GameObject cellGo = GetCellAtPos(InputHandler.Instance.mouseInWorld);
            if (cellGo == null) return;

            Cell cell = cellGo.GetComponent<Cell>();
            //Set this based on a number and what is selected
            cell.ChangeCellWalkalbeType(CellWalkableType.FullValid);
            cell.RoomNr = 2;

            OverrideSaveGrid(); //Works since we already just are changing the CurrentGrid in the GridManager
        }

        public void SetDefaultOnCell()
        {
            GameObject cellGo = GetCellAtPos(InputHandler.Instance.mouseInWorld);
            if (cellGo == null) return;

            Cell cell = cellGo.GetComponent<Cell>();
            //Set this based on a number and what is selected
            cell.RoomNr = -1;
            cell.ChangeCellWalkalbeType(CellWalkableType.NotValid);

            OverrideSaveGrid(); //Works since we already just are changing the CurrentGrid in the GridManager
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

        /*
         *         {
            Vector2 mouseInWorld = InputHandler.Instance.mouseInWorld;

            //Find grid if there is a tile under the mouse. with inputhandler mousepos in world
            // Check within grid.startpos.x + cell.dem * cell.scale and on the other side

            Grid grid = GridManager.Instance.CurrentGrid;

            if (grid == null) return;

            int scale = Cell.Demension * Cell.Scale;
            Rectangle gridSize = new((int)grid.StartPostion.X, (int)grid.StartPostion.Y, grid.Width * scale, grid.Height * scale);
            
            if (gridSize.Contains(mouseInWorld))
            {
                // Mouse inside grid
                GameObject cellGo = grid.GetCellGameObject(mouseInWorld);
                if (cellGo == null) return;

                Point cellGridPos = cellGo.Transform.GridPosition;
                grid.Cells[cellGridPos].GetComponent<SpriteRenderer>().Color = Color.Red;
                Cell cell = cellGo.GetComponent<Cell>();
                cell.CellWalkableType = CellWalkableType.FullValid;
                cell.RoomNr = 2;
                GridManager.Instance.OverrideSaveGrid(grid);
            }
         */

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

      

        private void OnGridIndexChanged()
        {
        }
    }
}