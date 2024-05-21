using FørsteÅrsEksamen.ComponentPattern.Classes;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    internal class MoveCmd : ICommand
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
            // Inputs maybe gets set weird if there is 3 or more velocities.
            // Maybe has something if the there is a lot. It gets set to Vector zero and adds.
            player.AddInput(velocity);
        }
    }
}