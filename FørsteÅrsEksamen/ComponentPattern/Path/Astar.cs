﻿using DoctorsDungeon.ComponentPattern.Enemies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoctorsDungeon.ComponentPattern.Path;

// Asser, Oscar
public class Astar
{
    private Dictionary<Point, GameObject> cells;
    private int gridDem;
    private HashSet<GameObject> open;
    private HashSet<GameObject> closed;
    private List<GameObject> path = new();
    private Enemy thisEnemy;
    private List<Enemy> otherEnemies; // So we can take into account the other enemies paths.
    public void Start(Enemy enemy, List<Enemy> enemyList)
    {
        otherEnemies = enemyList;
        this.thisEnemy = enemy; 

        gridDem = Cell.dimension * Cell.Scale;
        cells = GridManager.Instance.CurrentGrid.Cells.ToDictionary(
            entry => entry.Key,
            entry => entry.Value
        );
    }

    private Point start;

    public List<GameObject> FindPath(Point start, Point goal)
    {
        // Maybe check if the found path is the same as any of the others here.
        Point newPoint = GetNewPointIfOcupated();
        if (newPoint != new Point(-1, -1))
        {
            goal = newPoint;
        }

        this.start = start;
        ResetCells(); // Gets the Cells ready

        open = new HashSet<GameObject>(); // Cells to check
        closed = new HashSet<GameObject>(); // Checked cells
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

            // Checks if its on the Goal grid position, if it is, we have found the path
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
    /// <summary>
    /// Reverses the found path, by going though each GameObject and finding its Parent.
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    private List<GameObject> RetracePath(GameObject startPoint, GameObject endPoint)
    {
        path.Clear();
        GameObject currentNode = endPoint;

        int amount = 0;
        while (currentNode != startPoint && amount < 20)
        {
            amount++;

            if (currentNode == null)
            {
                continue;
            } 
            path.Add(currentNode);
            currentNode = currentNode.GetComponent<Cell>().Parent; 
        }

        path.Add(startPoint);
        path.Reverse();

        //foreach (GameObject go in path)
        //{
        //    go.GetComponent<SpriteRenderer>().Color = Color.Aqua;
        //}

        return path;
    }

    private void ResetCells()
    {
        foreach (GameObject cell in cells.Values)
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

    Random rnd = new();
    private Point GetNewPointIfOcupated()
    {
        Point retPoint = new Point(-1, -1);
        List<GameObject> availableTargets = new();

        bool shouldChange = false;
        foreach (Enemy otherEnemy in otherEnemies)
        {
            if (otherEnemy.TargetPoint != thisEnemy.TargetPoint) continue;
            shouldChange = true;
        }

        if (!shouldChange) return retPoint;

        List<GameObject> targetPointNeighbors = GetNeighbors(thisEnemy.TargetPoint);

        foreach (GameObject neighborGo in targetPointNeighbors)
        {
            bool cellBeingUsed = false;

            foreach (Enemy otherEnemy in otherEnemies)
            {
                if (neighborGo.Transform.GridPosition != otherEnemy.TargetPoint) continue;

                cellBeingUsed = true;
                break;
            }

            if (!cellBeingUsed) availableTargets.Add(neighborGo);
        }

        GameObject closestTarget = null;

        closestTarget = availableTargets[rnd.Next(0, availableTargets.Count)];

        retPoint = closestTarget.Transform.GridPosition;
        thisEnemy.TargetPoint = retPoint;

        return retPoint;
        // To select one of the targets, to spread out the enemies. Dosent take into account the classes
        // Also have the bug where if the target is on the other side of the player, it will stop inside the player
        // This is caused since the enemy only follows to a certain point and stop a bit before the end target. 

        // Need to find a way to stop the path from going though the point where the player is, 
        // since that causes the enemy to stop inside the player.
        // Could then search for distance or something.
        // The debug stuff(paths n stuff) is very lackluster too, so maybe there should be used some time for that?

        // There should also be something so if the enemy dosent move or something, it tries again to find a path
        // So it always are going to be after the player
    }
    //Vector2 thisPosition = thisEnemy.GameObject.Transform.Position;
    //float closestDistance = float.MaxValue;
    //foreach (GameObject availableTarget in availableTargets)
    //{
    //    float distance = Vector2.Distance(thisPosition, availableTarget.Transform.Position);
    //    if (distance < closestDistance)
    //    {
    //        closestDistance = distance;
    //        closestTarget = availableTarget;
    //    }
    //}

    // First check if the targetpath is the same as other enemy
    // get neighbors around of the normal targetpath.
    // Then take a distance check on the not used neighbors to find the closest one. 
    // If there is none neighbors just dont change the targetpath, so it stacks.
    // Lastly change the targetpath

    // Return newPoint(-1,-1) if there isnt a need to change the targetpath.

    // In enemy script after the astar, change the target point to the astar target point. 


    // Enemies and player should maybe have their sprite go up and down, or left and right when walking? 
    // and attacking. 

    // Should also have red color and maybe a outline that gets showed for a quick second?
    // For outline just use Asesprite, and either delete the normal sprite or have it black and white
    // Then when attacked, it changes sprite and the color of the sprite, and changes back to normal after

    // Like so it just gets drawn in the character when taking damage.
    // Could also just have a component that handles TakeDamage, and shows the outline? 


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