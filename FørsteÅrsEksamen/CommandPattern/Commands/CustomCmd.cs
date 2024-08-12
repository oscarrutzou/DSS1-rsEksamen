using System;

namespace DoctorsDungeon.CommandPattern.Commands;

// Oscar

// Still use the ICommand since it would take to much time to refactor
// if we wanted the InputHandler to both take ICommands and Actions.
// We therefore choose this approch to make a flexible approach -
// where we -can- use more complex commands, but also this cmd that is essentially a longer Action.
public class CustomCmd : Command
{
    private readonly Action action;

    public CustomCmd(Action action)
    {
        this.action = action;
    }

    public override void Execute()
    {
        action?.Invoke();
    }
}