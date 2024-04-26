using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.GameManagement;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.ComponentPattern.Grid
{
    public class GridManager
    {
        #region Parameters
        private readonly GridManager instance;

        public GridManager Instance
        { get { return instance ?? new GridManager(); } }

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

        public void InitalizeGrids()
        {
            GameObject go = new();
            go.AddComponent<Grid>("Bottom");
            GameWorld.Instance.Instantiate(go);
        }

        private void OnGridIndexChanged()
        {

        }
    }
}