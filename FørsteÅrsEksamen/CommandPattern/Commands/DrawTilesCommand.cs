using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Path;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    public class DrawTilesCommand : ICommand
    {

        public void Execute()
        {
            Vector2 mouseInWorld = InputHandler.Instance.mouseInWorld;

            //Find grid if there is a tile under the mouse. with inputhandler mousepos in world
            // Check within grid.startpos.x + cell.dem * cell.scale and on the other side

            Grid grid = GridManager.Instance.CurrentGrid;

            int scale = Cell.dimension * Cell.Scale;
            Rectangle gridSize = new((int)grid.StartPostion.X, (int)grid.StartPostion.Y, grid.Width * scale, grid.Height * scale);
            
            if (gridSize.Contains(mouseInWorld))
            {
                // Mouse inside grid
                GameObject cellGo = grid.GetCellGameObject(mouseInWorld);
                if (cellGo == null) return;

                Point cellGridPos = cellGo.Transform.GridPosition;
                grid.Cells[cellGridPos].GetComponent<SpriteRenderer>().Color = Color.Red;
                Cell cell = cellGo.GetComponent<Cell>();
                cell.CellWalkableType = CellWalkableType.FullValid;
                GridManager.Instance.SaveGrid(grid);
            }
        }

        //Make a mark in the right corner that just is a bool that check if there have been made any changes to the data (for debug) so we can save it.
        //Maybe make a ctrl z + x command. 
        // Multiple command inputs?
        // Make it save the new grid. 
        // Change it so the grid manager only shows 1 grid, since thats what our design is made.
        // Change all foreach to just check the grid != null. 
        // Make commen commands to the contains and stuff.

        /*
         * GameObject cellGo = GridManager.Instance.GetCellAtPos(mousePos);
            if (cellGo != null)
            {
                Vector2 cellPos = cellGo.Transform.Position;
                Point cellGridPos = cellGo.Transform.GridPosition;
         */
    }
}