using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    internal class QuitCommand : ICommand
    {
        public void Execute()
        {
            GameWorld.Instance.Exit();
        }
    }
}