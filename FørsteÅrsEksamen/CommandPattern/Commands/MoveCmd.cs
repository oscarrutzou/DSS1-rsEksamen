using DoctorsDungeon.ComponentPattern.PlayerClasses;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.CommandPattern.Commands;

// Oscar
public class MoveCmd : Command
{
    private Player player;
    private Vector2 velocity;

    public MoveCmd(Player player, Vector2 velocity)
    {
        this.player = player;
        this.velocity = velocity;
    }

    public override void Execute()
    {
        player.AddInput(velocity);
    }
}