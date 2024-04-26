using BaseProject.GameManagement;

namespace BaseProject.CommandPattern.Commands
{
    internal class QuitCommand : ICommand
    {
        public void Execute()
        {
            GameWorld.Instance.Exit();
        }
    }
}
