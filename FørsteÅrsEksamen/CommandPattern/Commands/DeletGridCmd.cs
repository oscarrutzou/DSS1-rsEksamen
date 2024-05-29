using DoctorsDungeon.ComponentPattern.Path;

namespace DoctorsDungeon.CommandPattern.Commands
{
    // Oscar
    public class DeletGridCmd : ICommand
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