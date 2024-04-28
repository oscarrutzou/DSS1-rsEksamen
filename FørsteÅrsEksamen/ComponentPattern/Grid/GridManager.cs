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


        #endregion

        IRepository repository;
        public void InitalizeGrids()
        {
            repository = FileRepository.Instance;
 
            string gridName = "Bottom";

            if (repository.DoesGridExist(gridName))
            {
                DeleteGrids();
                GameObject go = repository.GetGrid("Bottom");
                Grids.Add(go.GetComponent<Grid>());
            }
            else
            {
                GameObject gridGo = new();
                Grid grid = gridGo.AddComponent<Grid>("Bottom", new Vector2(-500, 0), 4, 4);
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