using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Grid;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    public class DrawTilesCommand : ICommand
    {
        private Grid grid;

        public DrawTilesCommand(Grid grid)
        {
            this.grid = grid;
        }

        public void Execute()
        {
            Vector2 mouseInWorld = InputHandler.Instance.mouseInWorld;

            //Find grid if there is a tile under the mouse. with inputhandler mousepos in world
            // Check within grid.startpos.x + cell.dem * cell.scale and on the other side
            foreach (Grid grid in GridManager.Instance.Grids)
            {
                Rectangle gridSize = new((int)grid.StartPostion.X, (int)grid.StartPostion.Y, grid.Width, grid.Height);
                if (gridSize.Contains(mouseInWorld))
                {
                    // Mouse inside grid
                    grid.Cells[new Point(0, 0)].GetComponent<SpriteRenderer>().Color = Color.Red;
                }
            }
        }
    }
}