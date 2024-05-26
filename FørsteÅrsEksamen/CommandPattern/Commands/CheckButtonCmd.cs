using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.GameManagement.Scenes;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DoctorsDungeon.CommandPattern.Commands
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