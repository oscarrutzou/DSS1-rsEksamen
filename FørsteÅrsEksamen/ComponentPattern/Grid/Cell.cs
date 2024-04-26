﻿using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.ComponentPattern.Grid
{
    public enum CellWalkableType
    {
        NotValid,
        FullValid,
    }

    // Oscar
    public class Cell : Component
    {
        public static int Demension = 16;
        public readonly static Vector2 ScaleSize = new(4, 4);

        // For the Astar algortihm
        public CellWalkableType CellWalkableType = CellWalkableType.NotValid;
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
            GameObject.Transform.Scale = ScaleSize;

            GameObject.Transform.Position = grid.StartPostion
                + new Vector2(point.X * Demension * ScaleSize.X + Demension * ScaleSize.X / 2,
                              point.Y * Demension * ScaleSize.Y + Demension * ScaleSize.Y / 2);
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