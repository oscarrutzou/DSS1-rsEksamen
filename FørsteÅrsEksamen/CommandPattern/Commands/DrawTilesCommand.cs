using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    public class DrawTilesCommand : ICommand
    {

        public void Execute()
        {
            Vector2 mouseInWorld = InputHandler.Instance.mouseInWorld;
            //Find grid if there is a tile under the mouse. with inputhandler mousepos in world
            //
        }
    }
}
