using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.GameManagement;
using FørsteÅrsEksamen.RepositoryPattern;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
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

        #endregion

        // Måske skal der ikke være en gridmanager siden der nok max vil være 1 grid på 
        public void InitalizeGrids()
        {
            repository = FileRepository.Instance; //Måske lav rep til en static som er den måde den save på, som bliver bestemt i starten.
            // Lav det til at alt er saved på pc og hvis timestamp er anderledet på postgre end file, skal den først uploade alt hvis den har adgang, før den starter?
            // Brug file system hvis der ikke er adgang til postgre
 
            string gridName = "Bottom";

            if (repository.DoesGridExist(gridName))
            {
                DeleteGrids();
                GameObject go = repository.GetGrid(gridName);
                Grids.Add(go.GetComponent<Grid>());
            }
            else
            {
                GameObject gridGo = new();
                Grid grid = gridGo.AddComponent<Grid>(gridName, new Vector2(-500, 0), 4, 4);
                grid.GenerateGrid();
                Grids.Add(grid);

                repository.SaveGrid(grid, "grid_1_" + grid.Description);
            }
        }

        public void LoadGrids()
        {
            DeleteGrids();
            GameObject go = repository.GetGrid("Bottom");
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