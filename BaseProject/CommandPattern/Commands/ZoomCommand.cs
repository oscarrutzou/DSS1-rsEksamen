

using BaseProject.GameManagement;
using Microsoft.Xna.Framework;

namespace BaseProject.CommandPattern.Commands
{
    internal class ZoomCommand : ICommand
    {
        private float zoomAmount;
        internal ZoomCommand(float zoomAmount) { 
            this.zoomAmount = zoomAmount;
        }

        public void Execute()
        {
            GameWorld.Instance.WorldCam.ChangeZoom(zoomAmount); // Only change zoom on worldCam
        }
    }
}
