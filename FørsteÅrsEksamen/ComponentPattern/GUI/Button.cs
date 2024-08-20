using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace DoctorsDungeon.ComponentPattern.GUI;

// Stefan
public class Button : Component
{
    #region Properties

    public Action OnClick;
    public string Text;
    private SpriteFont font;

    private SpriteRenderer spriteRenderer;
    private Collider collider;

    public Color TextColor;

    private Color baseColor;
    public Color OnHoverColor = new(200, 200, 200);
    public Color OnMouseDownColor = new(150, 150, 150);

    public Vector2 MaxScale { get; private set; }
    private bool invokeActionOnFullScale;
    private bool hasPressed;

    private Vector2 scaleUpAmount;
    private float scaleDownOnClickAmount = 0.95f;

    #endregion Properties

    public Button(GameObject gameObject) : base(gameObject)
    {
        MaxScale = GameObject.Transform.Scale;
        scaleUpAmount = new Vector2(MaxScale.X * 0.01f, MaxScale.Y * 0.01f);

        font = GlobalTextures.DefaultFont;
        GameObject.Type = GameObjectTypes.Gui;
    }

    public Button(GameObject gameObject, string text, bool invokeActionOnFullScale, Action onClick) : base(gameObject)
    {
        MaxScale = GameObject.Transform.Scale;
        scaleUpAmount = new Vector2(MaxScale.X * 0.01f, MaxScale.Y * 0.01f);

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

        TextColor = GameWorld.TextColor;
    }

    public override void Update()
    {
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
            Math.Min(MaxScale.X, scale.X + scaleUpAmount.X),
            Math.Min(MaxScale.Y, scale.Y + scaleUpAmount.Y));

        if (!GameObject.IsEnabled
            || !invokeActionOnFullScale
            || !hasPressed
            || GameObject.Transform.Scale != MaxScale) return;

        OnClick?.Invoke();
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
        MaxScale = scale;
        scaleUpAmount = new Vector2(MaxScale.X * 0.01f, MaxScale.Y * 0.01f);
    }

    public void OnClickButton()
    {
        if (!GameObject.IsEnabled) return;

        GameObject.Transform.Scale = new Vector2(
            MaxScale.X * scaleDownOnClickAmount,
            MaxScale.Y * scaleDownOnClickAmount);

        if (invokeActionOnFullScale)
            hasPressed = true;
        else
            OnClick?.Invoke();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // If the text is not visible or null, we don't need to do anything
        if (string.IsNullOrEmpty(Text)) return;

        GuiMethods.DrawTextCentered(spriteBatch, font, GameObject.Transform.Position, Text, BaseMath.TransitionColor(TextColor), Vector2.Zero);
    }
}