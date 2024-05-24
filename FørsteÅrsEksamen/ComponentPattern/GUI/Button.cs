using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace FørsteÅrsEksamen.ComponentPattern.GUI
{
    public class Button : Component
    {
        #region Properties

        public Action OnClick;
        public string Text;
        private SpriteFont font;

        private SpriteRenderer spriteRenderer;
        private Collider collider;

        public Color TextColor = Color.Black;

        private Color baseColor;
        public Color OnHoverColor = Color.Cyan;
        public Color OnMouseDownColor = Color.DarkCyan;

        private Vector2 maxScale;
        private float clickCooldown = 0.1f; // The delay between button clicks in seconds
        private float timeSinceLastClick = 0; // The time since the button was last clicked
        private bool invokeActionOnFullScale;
        private bool hasPressed;

        private Vector2 scaleUpAmount;
        private float scaleDownOnClickAmount = 0.95f;

        #endregion Properties

        public Button(GameObject gameObject) : base(gameObject)
        {
            maxScale = GameObject.Transform.Scale;
            scaleUpAmount = new Vector2(maxScale.X * 0.01f, maxScale.Y * 0.01f);

            font = GlobalTextures.DefaultFont;
            GameObject.Type = GameObjectTypes.Gui;
        }

        public Button(GameObject gameObject, string text, bool invokeActionOnFullScale, Action onClick) : base(gameObject)
        {
            maxScale = GameObject.Transform.Scale;
            scaleUpAmount = new Vector2(maxScale.X * 0.01f, maxScale.Y * 0.01f);

            font = GlobalTextures.DefaultFont;
            this.Text = text;
            this.invokeActionOnFullScale = invokeActionOnFullScale;
            this.OnClick = onClick;
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
            if (timeSinceLastClick < clickCooldown)
            {
                timeSinceLastClick += GameWorld.DeltaTime;
            }

            if (IsMouseOver())
            {
                if (InputHandler.Instance.MouseState.LeftButton != ButtonState.Released)
                {
                    spriteRenderer.Color = OnMouseDownColor;
                    return;
                }

                spriteRenderer.Color = OnHoverColor;
            }
            else
            {
                spriteRenderer.Color = baseColor;
            }

            Vector2 scale = GameObject.Transform.Scale;

            // Scales up too fast
            GameObject.Transform.Scale = new Vector2(
                Math.Min(maxScale.X, scale.X + scaleUpAmount.X),
                Math.Min(maxScale.Y, scale.Y + scaleUpAmount.Y));

            if (!GameObject.IsEnabled
                || !invokeActionOnFullScale
                || !hasPressed
                || GameObject.Transform.Scale != maxScale) return;

            OnClick?.Invoke();
            Debug.WriteLine("Invoke");

            hasPressed = false;
        }

        public bool IsMouseOver()
        {
            if (collider == null) return false;
            return collider.CollisionBox.Contains(InputHandler.Instance.MouseOnUI.ToPoint());
        }

        public void ChangeScale(Vector2 scale)
        {
            GameObject.Transform.Scale = scale;
            maxScale = scale;
            scaleUpAmount = new Vector2(maxScale.X * 0.01f, maxScale.Y * 0.01f);
        }

        public void OnClickButton()
        {
            if (!GameObject.IsEnabled) return;

            GameObject.Transform.Scale = new Vector2(
                maxScale.X * scaleDownOnClickAmount,
                maxScale.Y * scaleDownOnClickAmount);

            if (timeSinceLastClick < clickCooldown) return;

            timeSinceLastClick = 0;

            if (invokeActionOnFullScale)
            {
                hasPressed = true;
            }
            else
            {
                OnClick?.Invoke();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // If the text is not visible or null, we don't need to do anything
            if (string.IsNullOrEmpty(Text)) return;

            GuiMethods.DrawTextCentered(spriteBatch, font, GameObject.Transform.Position, Text, TextColor);
        }
    }
}