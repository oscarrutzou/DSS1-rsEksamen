using FørsteÅrsEksamen.ComponentPattern.Path;
using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    internal class QuitCmd : ICommand
    {
        private bool test;
        public void Execute()
        {
            test = !test;

            if (test)
            {
                GridManager.Instance.DeleteDrawnGrid();
            }else { 
                GridManager.Instance.LoadGrid("Test1");
            }
            //GameWorld.Instance.Exit();
        }
    }
}