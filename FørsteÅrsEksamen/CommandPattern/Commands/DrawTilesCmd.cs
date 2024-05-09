using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.Path;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    public class DrawTilesCmd : ICommand
    {

        public void Execute()
        {
            GridManager.Instance.DrawOnCells();
        }


        //Make a mark in the right corner that just is a bool that check if there have been made any changes to the data (for debug) so we can save it.
        //Maybe make a ctrl z + x command. 
        // Multiple command inputs?
        // Make it save the new grid. 
        // Change it so the grid manager only shows 1 grid, since thats what our design is made.
        // Change all foreach to just check the grid != null. 
        // Make commen commands to the contains and stuff.
    }
}