using ShamansDungeon.ComponentPattern.Enemies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShamansDungeon.ComponentPattern.Path;

// Asser, Oscar
public class Astar
{
    private Dictionary<Point, GameObject> _cells;
    private int _gridDem;
    private HashSet<GameObject> _open;
    private HashSet<GameObject> _closed;
    private List<GameObject> _path = new();
    private Enemy _thisEnemy;
    private List<Enemy> _otherEnemies; // So we can take into account the other enemies paths.

    private Point _start;
    private Random _rnd = new();

    public void Start(Enemy enemy, List<Enemy> enemyList)
    {
        _otherEnemies = enemyList;
        _thisEnemy = enemy;

        _gridDem = Cell.Dimension * Cell.Scale;
        _cells = GridManager.Instance.CurrentGrid.Cells.ToDictionary(
            entry => entry.Key,
            entry => entry.Value
        );
    }

    public void UpdateEnemyListReferences(List<Enemy> enemyList)
    {
        _otherEnemies = enemyList;
    }

    public List<GameObject> FindPath(Point start, Point goal)
    {
        // Shouldnt happen but still keep an eye out
        if (_cells == null || _cells.Count == 0) return null;

        // Check if the found path is the same as any of the others here.
        Point newPoint = GetNewPointIfOcupated();
        if (newPoint != new Point(-1, -1))
        {
            goal = newPoint;
        }
        this._start = start;

        ResetCells(); // Gets the Cells ready

        _open = new HashSet<GameObject>(); // Cells to check
        _closed = new HashSet<GameObject>(); // Checked cells
        if (!_cells.ContainsKey(start) || !_cells.ContainsKey(goal)) //Makes sure the cell start and end isnt the same
        {
            return null;
        }

        _open.Add(_cells[start]); // Starts with the start cell

        while (_open.Count > 0)
        {
            GameObject curCellGo = _open.First(); // Gets the first object
            foreach (GameObject cellGo in _open)
            {
                Cell cell = cellGo.GetComponent<Cell>();
                Cell curCell = curCellGo.GetComponent<Cell>();
                if (cell.F < curCell.F || cell.F == curCell.F && cell.H < curCell.H) // Takes the closest cell
                {
                    curCellGo = cellGo;
                }
            }

            // Check is complete, so we can move it from the unchecked HashSet to the checked.
            _open.Remove(curCellGo);
            _closed.Add(curCellGo);

            // Gets the Cell component
            Cell newCurCell = curCellGo.GetComponent<Cell>();

            // Checks if its on the Goal grid position, if it is, we have found the path
            if (newCurCell.GameObject.Transform.GridPosition.X == goal.X && newCurCell.GameObject.Transform.GridPosition.Y == goal.Y)
            {
                // Path found!
                return RetracePath(_cells[start], _cells[goal]);
            }

            // Gets neighbor GameObjects from the new postion, checks all 8 directions.
            List<GameObject> neighbors = GetNeighbors(newCurCell.GameObject.Transform.GridPosition);

            foreach (GameObject neighborGo in neighbors)
            {
                if (_closed.Contains(neighborGo)) continue; // Dont check objects that have been checked.

                // Finds the G cost from the start node to the current node
                //throw new Exception("Check the GetDistance to make sure it gets the start pos is used");
                int newMovementCostToNeighbor = newCurCell.G + newCurCell.cost + GetDistance(newCurCell.GameObject.Transform.GridPosition, newCurCell.GameObject.Transform.GridPosition);

                Cell neighbor = neighborGo.GetComponent<Cell>();

                // Updates the cost for the current position.
                if (!(newMovementCostToNeighbor < neighbor.G || !_open.Contains(neighborGo))) continue; // I know its a bad way with the !() inside a if, how could i fix this? The < works like that even if its not inverted with the !?

                neighbor.G = newMovementCostToNeighbor; // C
                                                        // Calulates H using manhatten principle
                neighbor.H = ((Math.Abs(neighbor.GameObject.Transform.GridPosition.X - goal.X) + Math.Abs(goal.Y - neighbor.GameObject.Transform.GridPosition.Y)) * 10);

                neighbor.Parent = curCellGo;

                if (!_open.Contains(neighborGo)) _open.Add(neighborGo);
            }
        }

        return null;
    }

    /// <summary>
    /// Reverses the found path, by going though each GameObject and finding its Parent.
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    private List<GameObject> RetracePath(GameObject startPoint, GameObject endPoint)
    {
        _path.Clear();
        GameObject currentNode = endPoint;

        int amount = 0;
        while (currentNode != startPoint && amount < 20)
        {
            amount++;

            if (currentNode == null)
            {
                continue;
            }
            _path.Add(currentNode);
            currentNode = currentNode.GetComponent<Cell>().Parent;
        }

        _path.Add(startPoint);
        _path.Reverse();

        return _path;
    }

    private void ResetCells()
    {
        foreach (GameObject cell in _cells.Values)
        {
            cell.GetComponent<Cell>().Parent = null;
        }
    }

    private int GetDistance(Point neighborgridPosition, Point endPoint)
    {
        int dstX = Math.Abs(neighborgridPosition.X - endPoint.X);
        int dstY = Math.Abs(neighborgridPosition.Y - endPoint.Y);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private Point GetNewPointIfOcupated()
    {
        Point retPoint = new Point(-1, -1);
        List<GameObject> availableTargets = new();

        bool shouldChange = false;

        foreach (Enemy otherEnemy in _otherEnemies)
        {
            if (otherEnemy.TargetPoint != _thisEnemy.TargetPoint
                && otherEnemy.GameObject.Transform.GridPosition != _thisEnemy.TargetPoint) continue;

            shouldChange = true;
            break;
        }

        if (!shouldChange) return retPoint;

        List<GameObject> targetPointNeighbors = GetNeighbors(_thisEnemy.TargetPoint);
        if (targetPointNeighbors.Count == 0) return retPoint; // Cant find new point

        foreach (GameObject neighborGo in targetPointNeighbors)
        {
            bool cellBeingUsed = false;

            foreach (Enemy otherEnemy in _otherEnemies)
            {
                if (neighborGo.Transform.GridPosition != otherEnemy.TargetPoint
                    && neighborGo.Transform.GridPosition != otherEnemy.GameObject.Transform.GridPosition) continue;
                
                cellBeingUsed = true;

                break;
            }

            if (!cellBeingUsed) availableTargets.Add(neighborGo);
        }

        if (availableTargets.Count == 0) return retPoint;

        GameObject closestTarget = null;
        // To select one of the targets, to spread out the enemies. Dosent take into account the classes
        closestTarget = availableTargets[_rnd.Next(0, availableTargets.Count)];

        //float closestDistance = 100000;
        //foreach (GameObject targetTile in availableTargets)
        //{
        //    float currentDistanceToThis = Vector2.Distance(_thisEnemy.GameObject.Transform.Position, targetTile.Transform.Position);

        //    if (currentDistanceToThis < closestDistance)
        //    {
        //        closestDistance = currentDistanceToThis;
        //        closestTarget = targetTile;
        //    }
        //}

        retPoint = closestTarget.Transform.GridPosition;
        _thisEnemy.TargetPoint = retPoint;

        return retPoint;
    }

    /// <summary>
    /// Gets the neighbors in all 8 directions from the point
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private List<GameObject> GetNeighbors(Point point)
    {
        // Could make this return alle neighbors inside a rectangle. Just the point then the amount.
        // Go to -amount, -amount, and check each towards amount x and y. Just not the same point
        List<GameObject> temp = new List<GameObject>();
        List<(int dx, int dy)> directions = new List<(int, int)>
        {
            (-1, 0), // Left
            (1, 0),  // Right
            (0, 1),  // Down
            (0, -1), // Up
            (-1, 1), // Left + Down
            (-1, -1), // Left + Up
            (1, 1), // Right + Down
            (1, -1) // Right + Up
        };
        foreach (var direction in directions)
        {
            int nx = point.X + direction.dx;
            int ny = point.Y + direction.dy;
            Point newPoint = new Point(nx, ny);

            //If the direction isn't in the grid or the point doesn't exist in the grid
            if (!(nx >= 0 && nx < _gridDem && ny >= 0 && ny < _gridDem) || !_cells.ContainsKey(newPoint)) continue;

            Cell newPointCell = _cells[newPoint].GetComponent<Cell>();
            if (newPointCell.CellWalkableType == CellWalkableType.NotValid 
                || newPointCell.CollisionNr == -1) continue;

            //Check if the cell is diagonally adjacent
            if (Math.Abs(point.X - nx) == 1 && Math.Abs(point.Y - ny) == 1)
            {
                // Check the cells directly to each side
                Point sidePoint1 = new Point(point.X, ny);
                Point sidePoint2 = new Point(nx, point.Y);

                // Does grid position exits in the grid
                if (!_cells.ContainsKey(sidePoint1) || !_cells.ContainsKey(sidePoint2)) continue;

                // To make sure the astar cant jump corner
                Cell side1Cell = _cells[sidePoint1].GetComponent<Cell>();
                Cell side2Cell = _cells[sidePoint2].GetComponent<Cell>();
                if (side1Cell.CellWalkableType == CellWalkableType.NotValid || side2Cell.CellWalkableType == CellWalkableType.NotValid) continue;
            }

            temp.Add(_cells[newPoint]);
        }
        return temp;
    }
}