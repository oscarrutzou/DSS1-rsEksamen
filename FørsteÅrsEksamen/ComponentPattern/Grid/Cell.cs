using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            // Centers the position of the cell.
            GameObject.Transform.Position = grid.StartPostion
                + new Vector2(point.X * Demension * ScaleSize.X + Demension * ScaleSize.X / 2,
                              point.Y * Demension * ScaleSize.Y + Demension * ScaleSize.Y / 2);
        }

        public Cell(GameObject gameObject, Grid grid, Point point, CellWalkableType type) : base(gameObject)
        {
            GameObject.Transform.GridPosition = point;
            GameObject.Transform.Scale = ScaleSize;
            this.CellWalkableType = type;

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

        public override void Draw(SpriteBatch spriteBatch)
        {
            GuiMethods.DrawTextCentered(spriteBatch, GlobalTextures.DefaultFont, GameWorld.Instance.WorldCam.zoom, GameObject.Transform.Position, "X", Color.Black);
        }
    }
}