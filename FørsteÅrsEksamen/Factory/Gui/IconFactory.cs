using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.Factory.Gui
{
    public static class IconFactory
    {
        public static GameObject CreateCursorIcon()
        {
            GameObject iconGo = new();
            iconGo.Type = GameObjectTypes.Gui;
            iconGo.Transform.Scale = new Vector2(3, 3);

            SpriteRenderer sr = iconGo.AddComponent<SpriteRenderer>();
            sr.SetLayerDepth(LayerDepth.Cursor);
            sr.SetSprite(TextureNames.MouseCursorDefault);
            sr.IsCentered = false;

            return iconGo;
        }
        public static GameObject CreateBackpackIcon()
        {
            GameObject iconGo = new();
            iconGo.Type = GameObjectTypes.Gui;

            iconGo.AddComponent<SpriteRenderer>();
            iconGo.AddComponent<BackpackIcon>();

            return iconGo;
        }

        public static GameObject CreateHourGlassIcon()
        {
            GameObject iconGo = new();
            iconGo.Type = GameObjectTypes.Gui;

            iconGo.AddComponent<Animator>();
            iconGo.AddComponent<SpriteRenderer>();
            iconGo.AddComponent<HourGlassIcon>();

            return iconGo;
        }
    }
}
