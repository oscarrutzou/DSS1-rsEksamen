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
    private SpriteFont _font;

    private SpriteRenderer _spriteRenderer;
    private Collider _collider;

    public Color TextColor;

    private Color _baseColor;
    public Color OnHoverColor = new(200, 200, 200);
    public Color OnMouseDownColor = new(150, 150, 150);

    public Vector2 MaxScale { get; private set; }
    private readonly bool _invokeActionOnFullScale;
    private bool _hasPressed;

    private Vector2 _scaleUpAmount;
    private readonly float _scaleDownOnClickAmount = 0.95f;
    private bool _hasPlayedHoverSound;
    #endregion Properties

    public Button(GameObject gameObject) : base(gameObject)
    {
        MaxScale = GameObject.Transform.Scale;
        _scaleUpAmount = new Vector2(MaxScale.X * 0.01f, MaxScale.Y * 0.01f);

        _font = GlobalTextures.DefaultFont;
        GameObject.Type = GameObjectTypes.Gui;
    }

    public Button(GameObject gameObject, string text, bool invokeActionOnFullScale, Action onClick) : base(gameObject)
    {
        MaxScale = GameObject.Transform.Scale;
        _scaleUpAmount = new Vector2(MaxScale.X * 0.01f, MaxScale.Y * 0.01f);

        _font = GlobalTextures.DefaultFont;
        this.Text = text;
        this._invokeActionOnFullScale = invokeActionOnFullScale;
        this.OnClick = onClick;
        GameObject.Type = GameObjectTypes.Gui;
    }

    public override void Start()
    {
        _collider = GameObject.GetComponent<Collider>();
        _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();

        _baseColor = _spriteRenderer.Color;

        TextColor = GameWorld.TextColor;
    }

    public override void Update()
    {
        if (IsMouseOver())
        {
            if (InputHandler.Instance.MouseState.LeftButton != ButtonState.Released)
            {
                _spriteRenderer.Color = OnMouseDownColor;
                return;
            }

            _spriteRenderer.Color = OnHoverColor;

            PlayHoverSound();
        }
        else
        {
            _spriteRenderer.Color = _baseColor;
            _hasPlayedHoverSound = false;
        }

        Vector2 scale = GameObject.Transform.Scale;

        // Scales up too fast
        GameObject.Transform.Scale = new Vector2(
            Math.Min(MaxScale.X, scale.X + _scaleUpAmount.X),
            Math.Min(MaxScale.Y, scale.Y + _scaleUpAmount.Y));

        if (!GameObject.IsEnabled
            || !_invokeActionOnFullScale
            || !_hasPressed
            || GameObject.Transform.Scale != MaxScale) return;

        OnClick?.Invoke();
        _hasPressed = false;
    }

    public bool IsMouseOver()
    {
        if (_collider == null) return false;
        return _collider.CollisionBox.Contains(InputHandler.Instance.MouseOnUI.ToPoint());
    }

    public void ChangeScale(Vector2 scale)
    {
        GameObject.Transform.Scale = scale;
        MaxScale = scale;
        _scaleUpAmount = new Vector2(MaxScale.X * 0.01f, MaxScale.Y * 0.01f);
    }

    public void OnClickButton()
    {
        if (!GameObject.IsEnabled) return;

        GameObject.Transform.Scale = new Vector2(
            MaxScale.X * _scaleDownOnClickAmount,
            MaxScale.Y * _scaleDownOnClickAmount);

        if (_invokeActionOnFullScale)
            _hasPressed = true;
        else
            OnClick?.Invoke();

        GlobalSounds.PlaySound(SoundNames.ButtonClicked, maxAmountPlaying: 5, soundVolume: 1, enablePitch: true);
    }

    private void PlayHoverSound()
    {
        if (_hasPlayedHoverSound) return;
        GlobalSounds.PlaySound(SoundNames.ButtonHover, maxAmountPlaying: 5, soundVolume: 0.7f, enablePitch: true);
        _hasPlayedHoverSound = true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // If the text is not visible or null, we don't need to do anything
        if (string.IsNullOrEmpty(Text)) return;

        GuiMethods.DrawTextCentered(spriteBatch, _font, GameObject.Transform.Position, Text, BaseMath.TransitionColor(TextColor));
    }
}