using FørsteÅrsEksamen.ComponentPattern.PlayerClasses;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    public class MoveCmd : ICommand
    {
        private Player player;
        private Vector2 velocity;

        public MoveCmd(Player player, Vector2 velocity)
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