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
            repository.SaveGrid(CurrentGrid);
        }

        public void LoadGrid(string gridName)
        {
            // Delete current drawn grid? Otherwise tru and 
            DeleteDrawnGrid();
            GameObject go = repository.GetGrid(gridName);
            CurrentGrid = go.GetComponent<Grid>();
        }


        public void DeleteDrawnGrid()
        {
            foreach (GameObject cellGo in CurrentGrid.Cells.Values)
            {
                GameWorld.Instance.Destroy(cellGo);
            }
            GameWorld.Instance.Destroy(CurrentGrid.GameObject);
            CurrentGrid = null;
        }


        public GameObject GetCellAtPos(Vector2 pos)
        {
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