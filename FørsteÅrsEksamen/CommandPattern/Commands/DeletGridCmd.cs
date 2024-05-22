using FørsteÅrsEksamen.ComponentPattern.Path;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    internal class DeletGridCmd : ICommand
    {
        private bool test;

        public void Execute()
        {
            test = !test;

            if (test)
            {
                GridManager.Instance.DeleteDrawnGrid();
            }
            else
            {
                GridManager.Instance.LoadGrid("Test1");
            }
        }
    }
}