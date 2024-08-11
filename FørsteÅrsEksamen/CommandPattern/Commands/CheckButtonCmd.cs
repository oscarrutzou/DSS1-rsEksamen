using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.GameManagement.Scenes;

namespace DoctorsDungeon.CommandPattern.Commands;

// Oscar
public class CheckButtonCmd : ICommand
{
    public void Execute()
    {
        foreach (GameObject gameObject in SceneData.GameObjectLists[GameObjectTypes.Gui])
        {
            if (gameObject.IsEnabled == false) continue;

            Button button = gameObject.GetComponent<Button>();

            if (button == null || !button.IsMouseOver()) continue;

            button.OnClickButton();
            break;
        }
    }
}