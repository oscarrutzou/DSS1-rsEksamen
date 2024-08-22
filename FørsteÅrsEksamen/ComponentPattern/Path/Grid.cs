using DoctorsDungeon.Factory;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Path;

// Oscar
public class Grid : Component
{
    #region Properties

    public string Name { get; set; }

    public Vector2 StartPostion { get; set; }

    public Dictionary<Point, GameObject> Cells { get; private set; } = new();
    // Evt. tag og lav en dict af en dict
    // Declare a dictionary to store multiple GameObjects per key
    public Dictionary<int, List<GameObject>> CellsCollisionDict = new Dictionary<int, List<GameObject>>();
    public List<Point> TargetPoints { get; private set; } = new List<Point>(); //Target cell points

    public int Width, Height;

    private readonly bool _isCentered = false;

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
        if (_isCentered)
        {
            StartPostion = new Vector2(
                StartPostion.X - (Width * Cell.Dimension * Cell.Scale / 2),
                StartPostion.Y - (Height * Cell.Dimension * Cell.Scale / 2)
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


    public List<GameObject> GetCellsInRadius(Vector2 pos, int searchRadius, int minimumCellDistance)
    {
        List<GameObject> tilesInRadius = new List<GameObject>();
        GameObject startCellGo = GetCellGameObject(pos) ?? throw new Exception("Not a tile under pos");

        Vector2 startPos = startCellGo.Transform.Position;
        Point startGridPos = startCellGo.Transform.GridPosition;

        // Calculate the range of positions that the radius covers
        Vector2 minPos = startPos - new Vector2(searchRadius);

        Vector2 maxPos = startPos + new Vector2(searchRadius);

        // Iterate over the positions in the range
        for (float x = minPos.X; x <= maxPos.X; x += Cell.Dimension * Cell.Scale)
        {
            for (float y = minPos.Y; y <= maxPos.Y; y += Cell.Dimension * Cell.Scale)
            {
                Vector2 gridPosVec = new(x, y);
                GameObject cellGo = GetCellGameObject(gridPosVec);
                if (cellGo == null) continue;

                Point gridPos = cellGo.Transform.GridPosition;

                if (gridPos == startGridPos) continue;

                // Check if the grid position is within the grid bounds
                if (gridPos.X >= 0 && gridPos.X < Width && gridPos.Y >= 0 && gridPos.Y < Height)
                {
                    Cell cell = cellGo.GetComponent<Cell>();

                    // Check if the tile is not null and not 'Empty'
                    if (cell != null && cell.CellWalkableType == CellWalkableType.FullValid)
                    {
                        Point pos1 = gridPos;
                        float dis = Distance(startPos, gridPosVec);

                        // Check if the position is within the radius from the center position
                        // gridPos - start pos >= minDis gridPos - startCellGo.Transform.GridPosition >= minimumCellDistance
                        if (dis <= searchRadius &&
                            gridPos.X - startCellGo.Transform.GridPosition.X >= minimumCellDistance &&
                            gridPos.Y - startCellGo.Transform.GridPosition.Y >= minimumCellDistance)
                        {
                            tilesInRadius.Add(cellGo);
                        }
                    }
                }
            }
        }

        return tilesInRadius;
    }

    public static float Distance(Vector2 value1, Vector2 value2)
    {
        float num = value1.X - value2.X;
        float num2 = value1.Y - value2.Y;
        return MathF.Sqrt(num * num + num2 * num2);
    }


    public GameObject GetCellGameObject(Vector2 pos)
    {
        if (pos.X < StartPostion.X || pos.Y < StartPostion.Y)
        {
            return null; // Position is negative, otherwise it will make a invisible tile in the debug, since it cast to int, then it gets rounded to 0 and results in row and column
        }

        // Calculates the position of each point. Maybe remove the zoom
        int gridX = (int)((pos.X - StartPostion.X) / (Cell.Dimension * Cell.Scale * GameWorld.Instance.WorldCam.zoom));
        int gridY = (int)((pos.Y - StartPostion.Y) / (Cell.Dimension * Cell.Scale * GameWorld.Instance.WorldCam.zoom));

        // Checks if its inside the grid.
        if (0 <= gridX && gridX < Width && 0 <= gridY && gridY < Height)
        {
            return Cells[new Point(gridX, gridY)];
        }

        return null; // Position is out of bounds
    }

    public Vector2 PosFromGridPos(Point point) => Cells[point].Transform.Position;

    public GameObject GetCellGameObjectFromPoint(Point point) => GetCellGameObject(PosFromGridPos(point));

    public Cell GetCellFromPos(Vector2 pos) => GetCellGameObject(pos).GetComponent<Cell>();

    public Cell GetCellFromPoint(Point point) => GetCellGameObjectFromPoint(point).GetComponent<Cell>();
}