using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.GameManagement;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    internal class CheckButtonCmd : ICommand
    {
        public void Execute()
        {
            foreach (GameObject gameObject in SceneData.GameObjectLists[GameObjectTypes.Gui])
            {
                if (gameObject.IsEnabled == false) continue;

                Button button = gameObject.GetComponent<Button>();

                if (button == null
                    || !button.IsMouseOver()) continue;

                button.OnClickButton();
                return;
            }
        }
    }
}