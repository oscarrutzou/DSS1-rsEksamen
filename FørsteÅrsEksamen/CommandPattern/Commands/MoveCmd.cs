using DoctorsDungeon.ComponentPattern.PlayerClasses;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.CommandPattern.Commands;

// Oscar
public class MoveCmd : Command
{
    private Player _player;
    private Vector2 _velocity;

    public MoveCmd(Player player, Vector2 velocity)
    {
        this._player = player;
        this._velocity = velocity;
    }

    public override void Execute()
    {
        _player.AddInput(_velocity);
    }
}