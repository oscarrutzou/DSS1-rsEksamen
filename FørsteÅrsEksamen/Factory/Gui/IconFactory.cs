using ShamansDungeon.ComponentPattern;
using ShamansDungeon.ComponentPattern.GUI;
using ShamansDungeon.GameManagement;
using Microsoft.Xna.Framework;

namespace ShamansDungeon.Factory.Gui
{
    public static class IconFactory
    {
        public static GameObject CreateCursorIcon()
        {
            GameObject iconGo = new(){
                Type = GameObjectTypes.Gui
            };
            iconGo.Transform.Scale = new Vector2(3, 3);

            SpriteRenderer sr = iconGo.AddComponent<SpriteRenderer>();
            sr.SetLayerDepth(LayerDepth.Cursor);
            sr.SetSprite(TextureNames.MouseCursorDefault);
            sr.IsCentered = false;

            GameObject cooldownBar = ScalableBarFactory.CreateCooldownBar();
            MouseIcon mouseIcon = iconGo.AddComponent<MouseIcon>();
            mouseIcon.SetMouseCooldownBar(cooldownBar);

            GameObject itemText = new() { 
                Type = GameObjectTypes.Gui
            };
            itemText.AddComponent<SpriteRenderer>();
            itemText.AddComponent<MouseItemText>();
            itemText.AddComponent<Collider>();
            mouseIcon.SetMouseItemText(itemText);

            return iconGo;
        }



        public static GameObject CreateBackpackIcon()
        {
            GameObject iconGo = new()
            {
                Type = GameObjectTypes.Gui
            };

            iconGo.AddComponent<SpriteRenderer>();
            iconGo.AddComponent<BackpackIcon>();
            iconGo.AddComponent<Collider>();

            return iconGo;
        }

        public static GameObject CreateHourGlassIcon()
        {
            GameObject iconGo = new()
            {
                Type = GameObjectTypes.Gui
            };

            iconGo.AddComponent<Animator>();
            iconGo.AddComponent<SpriteRenderer>();
            iconGo.AddComponent<HourGlassIcon>();

            return iconGo;
        }
    }
}
