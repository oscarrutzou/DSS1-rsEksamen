using FørsteÅrsEksamen.ComponentPattern.Grid;
using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    internal class QuitCommand : ICommand
    {
        private bool test;
        public void Execute()
        {
            test = !test;

            if (test)
            {
                GridManager.Instance.DeleteGrids();
            }else { 
                GridManager.Instance.LoadGrid("Test1");
            }
            //GameWorld.Instance.Exit();
        }
    }
}