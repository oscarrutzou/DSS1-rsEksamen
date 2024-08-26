using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.GameManagement.Scenes;

namespace DoctorsDungeon.CommandPattern.Commands;

// Oscar
public class CheckButtonCmd : Command
{
    private const double _clickCooldown = 0.2f; // The delay between button clicks in seconds
    private static double _timeSinceLastClick = 0;     // The time since the button was last clicked

    public CheckButtonCmd()
    {
        _timeSinceLastClick = _clickCooldown;
    }

    public override void Update()
    {
        if (_timeSinceLastClick < _clickCooldown)
            _timeSinceLastClick += GameWorld.DeltaTime;
    }

    public override void Execute()
    {
        if (_timeSinceLastClick < _clickCooldown) return;

        foreach (GameObject gameObject in SceneData.GameObjectLists[GameObjectTypes.Gui])
        {
            if (gameObject.IsEnabled == false) continue;

            Button button = gameObject.GetComponent<Button>();

            if (button == null || !button.IsMouseOver()) continue;

            button.OnClickButton();

            _timeSinceLastClick = 0;
            break;
        }
    }
}