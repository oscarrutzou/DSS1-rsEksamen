using FørsteÅrsEksamen.CompositPattern.Characters;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    internal class MoveCommand : ICommand
    {
        private Player player;
        private Vector2 velocity;

        public MoveCommand(Player player, Vector2 velocity)
        {
            this.player = player;
            this.velocity = velocity;
        }

        public void Execute()
        {
            player.AddInput(velocity);
        }
    }
}