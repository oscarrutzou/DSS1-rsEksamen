using Microsoft.Xna.Framework;

namespace BaseProject.CompositPattern.Grid
{
    public class Cell : Component
    {
        public static int demension = 16;
        public static Vector2 scaleSize = new(4, 4);
        public bool isValid = true;

        // For the Astar algortihm
        public int cost = 1;
        public int G;
        public int H;
        public int F => G + H;

        /// <summary>
        /// Parent is for the Astar, not the GameObject that is attached as "GameObject".
        /// </summary>
        public GameObject Parent { get; set; }


        public Cell(GameObject gameObject, Grid grid, Point point) : base(gameObject)
        {
            GameObject.Transform.GridPosition = point;
            GameObject.Transform.Scale = scaleSize;

            GameObject.Transform.Position = grid.StartPostion 
                + new Vector2(point.X * demension * scaleSize.X + demension * scaleSize.X / 2, 
                              point.Y * demension * scaleSize.Y + demension * scaleSize.Y / 2);
        }

        /// <summary>
        /// Resets the cell, to make it ready for another path.
        /// </summary>
        public void Reset()
        {
            Parent = null;
            G = 0;
            H = 0;
        }

    }
}
