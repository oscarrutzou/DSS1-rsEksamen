using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.CommandPattern.Commands;
using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.Factory.Gui;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Menus
{
    public class MainMenu : Scene
    {
        public override void Initialize()
        {
            InitButtons();
            InitCommands();
        }

        private void InitButtons()
        {
            GameObject startBtn = ButtonFactory.Create("Start Game", true, () => { });
            //GameWorld.Instance.ChangeScene(ScenesNames.OscarTestScene);
            GameWorld.Instance.Instantiate(startBtn);
        }

        private void InitCommands()
        {
            InputHandler.Instance.AddMouseUpdateCommand(MouseCmdState.Left, new CheckButtonCmd());
        }
    }
}