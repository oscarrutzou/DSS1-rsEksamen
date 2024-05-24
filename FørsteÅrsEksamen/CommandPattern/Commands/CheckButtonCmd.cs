using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.GameManagement;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FørsteÅrsEksamen.CommandPattern.Commands
{
    public class CheckButtonCmd : ICommand
    {

        public void Execute()
        {
            List<GameObject> gameObjectListCopy = new(SceneData.GameObjectLists[GameObjectTypes.Gui]);

            Action clickAction = null;

            foreach (GameObject gameObject in gameObjectListCopy)
            {
                if (gameObject.IsEnabled == false) continue;

                Button button = gameObject.GetComponent<Button>();

                if (button == null || !button.IsMouseOver()) continue;

                clickAction = button.OnClickButton;
                break;
            }

            clickAction?.Invoke();
        }
    }
}