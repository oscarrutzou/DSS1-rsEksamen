using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FørsteÅrsEksamen.ComponentPattern.GUI
{
    public class Button : Component
    {
        public Action OnClick;
        public string Text;
        public Color TextColor;
        public SpriteFont Font;

        public Button(GameObject gameObject) : base(gameObject)
        {
            Font = GlobalTextures.DefaultFont;
            GameObject.Type = GameObjectTypes.Gui;
        }

        public Button(GameObject gameObject, string text, Action onClick) : base(gameObject)
        {
            Font = GlobalTextures.DefaultFont;
            Text = text;
            OnClick = onClick;
            GameObject.Type = GameObjectTypes.Gui;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // If the text is not visible or null, we don't need to do anything
            if (string.IsNullOrEmpty(Text) || GameObject.GetComponent<SpriteRenderer>().ShouldDraw) return;
            GuiMethods.DrawTextCentered(spriteBatch, Font, GameWorld.Instance.UiCam.zoom, GameObject.Transform.Position, Text, TextColor);
        }
    }
}