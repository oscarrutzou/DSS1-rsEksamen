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
        private string text;
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
            this.text = text;
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

            if (InputHandler.Instance.MouseState.LeftButton != ButtonState.Released)
            {
                spriteRenderer.Color = OnMouseDownColor;
                return;
            }
            else if (IsMouseOver())
            {
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
            return collider.CollisionBox.Contains(InputHandler.Instance.MouseOnUI.ToPoint());
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
            if (string.IsNullOrEmpty(text) || !GameObject.GetComponent<SpriteRenderer>().ShouldDraw) return;

            GuiMethods.DrawTextCentered(spriteBatch, font, GameWorld.Instance.UiCam.zoom, GameObject.Transform.Position, text, TextColor);
        }
    }
}