using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.CompositPattern.Grid
{
    // Oscar
    public class Grid : Component
    {
        #region Properties

        public Vector2 StartPostion { get; set; }

        public Dictionary<Point, GameObject> Cells { get; private set; } = new Dictionary<Point, GameObject>();
        public List<Point> TargetPoints { get; private set; } = new List<Point>(); //Target cell points
        private int width, height;

        public int mapW, mapH; // To use when we have a larger map than collision

        private bool isCentered = true;

        private int demension => Cell.demension;

        public Grid(GameObject gameObject) : base(gameObject)
        {
        }

        #endregion Properties

        /// <summary>
        /// Generates a grid with GameObject Cells foreach node
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void GenerateGrid(Vector2 startPos, int width, int height)
        {
            #region Set Params

            this.width = width;
            this.height = height;

            if (isCentered)
            {
                startPos = new Vector2(
                    startPos.X - (width * demension * Cell.scaleSize.X / 2),
                    startPos.Y - (height * demension * Cell.scaleSize.Y / 2)
                );
            }

            this.StartPostion = startPos;

            #endregion Set Params

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Point point = new Point(x, y);
                    GameObject cellGo = new GameObject();
                    cellGo.AddComponent<Cell>(this, point);
                    cellGo.Type = GameObjectTypes.Cell;
                    SpriteRenderer sr = cellGo.AddComponent<SpriteRenderer>();
                    sr.SetLayerDepth(LAYERDEPTH.WorldBackground);
                    sr.SetSprite(TextureNames.Cell);

                    if ((x + y) % 2 == 0) sr.Color = new Color(30, 150, 20); // Set color so every second one is colored

                    Cells.Add(point, cellGo);
                    GameWorld.Instance.Instantiate(cellGo);
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
            int gridX = (int)((pos.X - StartPostion.X) / (Cell.demension * Cell.scaleSize.X * GameWorld.Instance.WorldCam.zoom));
            int gridY = (int)((pos.Y - StartPostion.Y) / (Cell.demension * Cell.scaleSize.Y * GameWorld.Instance.WorldCam.zoom));

            // Checks if its inside the grid.
            if (0 <= gridX && gridX < width && 0 <= gridY && gridY < height)
            {
                return Cells[new Point(gridX, gridY)];
            }

            return null; // Position is out of bounds
        }

        public Vector2 PosFromGridPos(Point point) => Cells[point].Transform.Position;

        public GameObject GetCellGameObjectFromPoint(Point point) => GetCellGameObject(PosFromGridPos(point));

        public Cell GetCellFromPoint(Point point) => GetCellGameObjectFromPoint(point).GetComponent<Cell>();
    }
}