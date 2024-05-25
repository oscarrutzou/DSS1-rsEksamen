using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FørsteÅrsEksamen.ComponentPattern.Path
{
    // Oscar
    public class Astar
    {
        private Dictionary<Point, GameObject> cells;
        private Grid grid;
        private int gridDem;
        private HashSet<GameObject> open;
        private HashSet<GameObject> closed;

        public List<GameObject> FindPath(Point start, Point goal)
        {
            gridDem = Cell.dimension * Cell.Scale;
            grid = GridManager.Instance.CurrentGrid;
            cells = grid.Cells; // Assign existing grid

            ResetCells(); // Gets the Cells ready

            open = new HashSet<GameObject>(); // Cells to check
            closed = new HashSet<GameObject>(); // Checkes cells
            if (!cells.ContainsKey(start) || !cells.ContainsKey(goal)) //Makes sure the cell start and end isnt the same
            {
                return null;
            }

            open.Add(cells[start]); // Starts with the start cell

            while (open.Count > 0)
            {
                GameObject curCellGo = open.First(); // Gets the first object
                foreach (GameObject cellGo in open)
                {
                    Cell cell = cellGo.GetComponent<Cell>();
                    Cell curCell = curCellGo.GetComponent<Cell>();
                    if (cell.F < curCell.F || cell.F == curCell.F && cell.H < curCell.H) // Takes the closest cell
                    {
                        curCellGo = cellGo;
                    }
                }

                // Check is complete, so we can move it from the unchecked HashSet to the checked.
                open.Remove(curCellGo);
                closed.Add(curCellGo);

                // Gets the Cell component
                Cell newCurCell = curCellGo.GetComponent<Cell>();

                // Checks if its on the Goal grid position, if its is, we have found the path
                if (newCurCell.GameObject.Transform.GridPosition.X == goal.X && newCurCell.GameObject.Transform.GridPosition.Y == goal.Y)
                {
                    // Path found!
                    return RetracePath(cells[start], cells[goal]);
                }

                // Gets neighbor GameObjects from the new postion, checks all 8 directions.
                List<GameObject> neighbors = GetNeighbors(newCurCell.GameObject.Transform.GridPosition);

                foreach (GameObject neighborGo in neighbors)
                {
                    if (closed.Contains(neighborGo)) continue; // Dont check objects that have been checked.

                    // Finds the G cost from the start node to the current node
                    //throw new Exception("Check the GetDistance to make sure it gets the start pos is used");
                    int newMovementCostToNeighbor = newCurCell.G + newCurCell.cost + GetDistance(newCurCell.GameObject.Transform.GridPosition, newCurCell.GameObject.Transform.GridPosition);

                    Cell neighbor = neighborGo.GetComponent<Cell>();

                    // Updates the cost for the current position.
                    if (newMovementCostToNeighbor < neighbor.G || !open.Contains(neighborGo))
                    {
                        neighbor.G = newMovementCostToNeighbor; // C
                        // Calulates H using manhatten principle
                        neighbor.H = ((Math.Abs(neighbor.GameObject.Transform.GridPosition.X - goal.X) + Math.Abs(goal.Y - neighbor.GameObject.Transform.GridPosition.Y)) * 10);

                        neighbor.Parent = curCellGo;

                        if (!open.Contains(neighborGo))
                        {
                            open.Add(neighborGo);
                        }
                    }
                }
            }

            return null;
        }

        /*
        Can also use the Euclidean distance to get the H
        float first = Math.Abs(end.gridPosition.X - next.gridPosition.X);
        float second = Math.Abs(end.gridPosition.Y - next.gridPosition.Y);
        float priority = newCost + (float)Math.Sqrt(Math.Pow(first, 2) + Math.Pow(second, 2)); // Euclidean distance
        */

        /// <summary>
        /// Reverses the found path, by going though each GameObject and finding its Parent.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private List<GameObject> RetracePath(GameObject startPoint, GameObject endPoint)
        {
            List<GameObject> path = new List<GameObject>();
            GameObject currentNode = endPoint;

            while (currentNode != startPoint)
            {
                if (currentNode == null) continue;
                path.Add(currentNode);
                currentNode = currentNode.GetComponent<Cell>().Parent;
            }
            path.Add(startPoint);
            path.Reverse();

            foreach (GameObject go in path)
            {
                go.GetComponent<SpriteRenderer>().Color = Color.Aqua;
            }

            return path;
        }

        private void ResetCells()
        {
            foreach (GameObject cell in grid.Cells.Values)
            {
                //cell.Color = cell.baseColor;
                //cell.GetComponent<SpriteRenderer>().
                //cell.Discovered = false;
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

        /// <summary>
        /// Gets the neighbors in all 8 directions from the point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private List<GameObject> GetNeighbors(Point point)
        {
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
                if (!(nx >= 0 && nx < gridDem && ny >= 0 && ny < gridDem) || !cells.ContainsKey(newPoint)) continue;

                Cell newPointCell = cells[newPoint].GetComponent<Cell>();
                if (newPointCell.CellWalkableType == CellWalkableType.NotValid) continue;

                //Check if the cell is diagonally adjacent
                if (Math.Abs(point.X - nx) == 1 && Math.Abs(point.Y - ny) == 1)
                {
                    // Check the cells directly to each side
                    Point sidePoint1 = new Point(point.X, ny);
                    Point sidePoint2 = new Point(nx, point.Y);

                    // Does grid position exits in the grid
                    if (!cells.ContainsKey(sidePoint1) || !cells.ContainsKey(sidePoint2)) continue;

                    // To make sure the astar cant jump corner
                    Cell side1Cell = cells[sidePoint1].GetComponent<Cell>();
                    Cell side2Cell = cells[sidePoint2].GetComponent<Cell>();
                    if (side1Cell.CellWalkableType == CellWalkableType.NotValid || side2Cell.CellWalkableType == CellWalkableType.NotValid) continue;
                }

                temp.Add(cells[newPoint]);
                //cells[newPoint].color = searchedColor;
            }
            return temp;
        }
    }
}