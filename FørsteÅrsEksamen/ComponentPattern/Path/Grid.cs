using DoctorsDungeon.Factory;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Path
{
    // Oscar
    public class Grid : Component
    {
        #region Properties

        public string Name { get; set; }

        public Vector2 StartPostion { get; set; }

        public Dictionary<Point, GameObject> Cells { get; private set; } = new Dictionary<Point, GameObject>();
        public List<Point> TargetPoints { get; private set; } = new List<Point>(); //Target cell points

        public int Width, Height;

        private readonly bool isCentered = false;

        public Grid(GameObject gameObject) : base(gameObject)
        {
        }

        public Grid(GameObject gameObject, string description, Vector2 startPos, int width, int height) : base(gameObject)
        {
            this.Name = description;
            this.StartPostion = startPos;
            this.Width = width;
            this.Height = height;
        }

        #endregion Properties

        /// <summary>
        /// Generates a grid with GameObject Cells foreach node
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void GenerateGrid()
        {
            Cells.Clear();
            if (isCentered)
            {
                StartPostion = new Vector2(
                    StartPostion.X - (Width * Cell.dimension * Cell.Scale / 2),
                    StartPostion.Y - (Height * Cell.dimension * Cell.Scale / 2)
                );
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Point point = new(x, y);
                    GameObject cellGo = CellFactory.Create(this, point);
                    Cells.Add(point, cellGo);
                }
            }
        }

        public GameObject GetCellGameObject(Vector2 pos)
        {
            if (pos.X < StartPostion.X || pos.Y < StartPostion.Y)
            {
                return null; // Position is negative, otherwise it will make a invisible tile in the debug, since it cast to int, then it gets rounded to 0 and results in row and column
            }

            // Calculates the position of each point. Maybe remove the zoom
            int gridX = (int)((pos.X - StartPostion.X) / (Cell.dimension * Cell.Scale * GameWorld.Instance.WorldCam.zoom));
            int gridY = (int)((pos.Y - StartPostion.Y) / (Cell.dimension * Cell.Scale * GameWorld.Instance.WorldCam.zoom));

            // Checks if its inside the grid.
            if (0 <= gridX && gridX < Width && 0 <= gridY && gridY < Height)
            {
                return Cells[new Point(gridX, gridY)];
            }

            return null; // Position is out of bounds
        }

        //public List<GameObject>

        public Vector2 PosFromGridPos(Point point) => Cells[point].Transform.Position;

        public GameObject GetCellGameObjectFromPoint(Point point) => GetCellGameObject(PosFromGridPos(point));

        public Cell GetCellFromPos(Vector2 pos) => GetCellGameObject(pos).GetComponent<Cell>();
        public Cell GetCellFromPoint(Point point) => GetCellGameObjectFromPoint(point).GetComponent<Cell>();
    }
}