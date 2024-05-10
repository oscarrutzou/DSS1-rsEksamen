using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FørsteÅrsEksamen.ComponentPattern.Path
{
    public enum CellWalkableType
    {
        NotValid,
        FullValid,
    }

    // Oscar
    public class Cell : Component
    {
        public static int dimension = 16;
        //public static readonly Vector2 ScaleSize = new(4, 4);
        public static int Scale = 4;

        private SpriteRenderer spriteRenderer;

        /// <summary>
        /// Used when selecting which room is active on each grid. Base is -1, so they dont count as a room
        /// </summary>
        public int RoomNr { get; set; } = -1;

        // For the Astar algortihm
        public CellWalkableType CellWalkableType;

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
            GameObject.Transform.Scale = new(Scale, Scale);

            CellWalkableType = CellWalkableType.NotValid;

            // Centers the position of the cell.
            GameObject.Transform.Position = grid.StartPostion
                + new Vector2(point.X * dimension * Scale + dimension * Scale / 2,
                              point.Y * dimension * Scale + dimension * Scale / 2);
        }

        public Cell(GameObject gameObject, Grid grid, Point point, CellWalkableType type) : base(gameObject)
        {
            GameObject.Transform.GridPosition = point;
            GameObject.Transform.Scale = new(Scale, Scale);

            CellWalkableType = type;

            // Centers the position of the cell.
            GameObject.Transform.Position = grid.StartPostion
                + new Vector2(point.X * dimension * Scale + dimension * Scale / 2,
                              point.Y * dimension * Scale + dimension * Scale / 2);
        }

        public Cell(GameObject gameObject, Grid grid, Point point, CellWalkableType type, int roomNr) : base(gameObject)
        {
            GameObject.Transform.GridPosition = point;
            GameObject.Transform.Scale = new(Scale, Scale);

            CellWalkableType = type;
            this.RoomNr = roomNr;

            // Centers the position of the cell.
            GameObject.Transform.Position = grid.StartPostion
                + new Vector2(point.X * dimension * Scale + dimension * Scale / 2,
                              point.Y * dimension * Scale + dimension * Scale / 2);
        }

        public override void Start()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) throw new System.Exception("Cell need a spriteRenderer");

            ChangeCellWalkalbeType(CellWalkableType); //Just the same here, so it turns the correct color.
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
            GuiMethods.DrawTextCentered(spriteBatch, GlobalTextures.DefaultFont, GameWorld.Instance.WorldCam.zoom, GameObject.Transform.Position, RoomNr.ToString(), Color.HotPink);
        }

        public void ChangeCellWalkalbeType(CellWalkableType cellWalkableType)
        {
            if (RoomNr == -1) spriteRenderer.ShouldDraw = false;
            else spriteRenderer.ShouldDraw = true;

            CellWalkableType = cellWalkableType;

            switch (CellWalkableType)
            { 
                case CellWalkableType.FullValid:
                    spriteRenderer.Color = Color.DarkOliveGreen;
                    break;
            }
        }


    }
}