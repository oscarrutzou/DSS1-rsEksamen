using FørsteÅrsEksamen.CommandPattern;
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
        public Color TextColor = Color.Black;
        public SpriteFont Font;

        private SpriteRenderer spriteRenderer;
        private Collider collider;
        private Color baseColor;
        private Color onHoverColor = Color.Cyan;
        private Color onMouseDownColor = Color.DarkCyan;

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

        public override void Start()
        {
            collider = GameObject.GetComponent<Collider>();
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            baseColor = spriteRenderer.Color;
        }

        public override void Update(GameTime gameTime)
        {
            if (collider.CollisionBox.Contains(InputHandler.Instance.mouseOnUI))
            {
                spriteRenderer.Color = onHoverColor;
            }
            else
            {
                spriteRenderer.Color = baseColor;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // If the text is not visible or null, we don't need to do anything
            if (string.IsNullOrEmpty(Text) || !GameObject.GetComponent<SpriteRenderer>().ShouldDraw) return;
            GuiMethods.DrawTextCentered(spriteBatch, Font, GameWorld.Instance.UiCam.zoom, GameObject.Transform.Position, Text, TextColor);
        }
    }
}