using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.GameManagement.Scenes;

namespace DoctorsDungeon.CommandPattern.Commands;

// Oscar
public class CheckButtonCmd : Command
{
    private const double clickCooldown = 0.1f; // The delay between button clicks in seconds
    private double timeSinceLastClick = 0;     // The time since the button was last clicked

    public CheckButtonCmd()
    {
        timeSinceLastClick = clickCooldown;
    }

    public override void Update()
    {
        if (timeSinceLastClick < clickCooldown)
            timeSinceLastClick += GameWorld.DeltaTime;
    }

    public override void Execute()
    {
        if (timeSinceLastClick < clickCooldown) return;

        foreach (GameObject gameObject in SceneData.GameObjectLists[GameObjectTypes.Gui])
        {
            if (gameObject.IsEnabled == false) continue;

            Button button = gameObject.GetComponent<Button>();

            if (button == null || !button.IsMouseOver()) continue;

            button.OnClickButton();

            timeSinceLastClick = 0;
            break;
        }
    }
}