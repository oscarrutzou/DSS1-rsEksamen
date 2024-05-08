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

        public List<Grid> Grids = new();
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
            if (repository.DoesGridExist(grid.Name))
            {
                GameObject go = repository.GetGrid(grid.Name);
                Grids.Add(go.GetComponent<Grid>());
                Grids.Add(grid);
            }
            else
            {
                grid.GenerateGrid();
                repository.SaveGrid(grid);
            }
        }

        public void LoadGrid(string gridName)
        {
            // Delete current drawn grid? Otherwise tru and 
            DeleteGrids();
            GameObject go = repository.GetGrid(gridName);
            Grids.Add(go.GetComponent<Grid>());
        }


        public void DeleteGrids()
        {
            foreach (Grid grid in Grids)
            {
                foreach (GameObject cellGo in grid.Cells.Values)
                {
                    GameWorld.Instance.Destroy(cellGo);
                }
                GameWorld.Instance.Destroy(grid.GameObject);
            }
            Grids.Clear();
        }



        private void OnGridIndexChanged()
        {
        }
    }
}