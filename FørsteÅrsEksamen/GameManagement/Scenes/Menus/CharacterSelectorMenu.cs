using FørsteÅrsEksamen.ComponentPattern;
using FørsteÅrsEksamen.ComponentPattern.GUI;
using FørsteÅrsEksamen.Factory.Gui;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.GameManagement.Scenes.Menus
{
    public class CharacterSelectorMenu : MenuScene
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void InitSettingsMenu()
        {

        }

        protected override void InitStartMenu()
        {
            GameObject btn = ButtonFactory.Create("XOXOX", true, () => { });
            btn.GetComponent<Button>().ChangeScale(new Vector2(7.5f, 15f));
            GameWorld.Instance.Instantiate(btn);
        }
    }
}
