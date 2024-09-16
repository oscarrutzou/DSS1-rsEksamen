using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Text;

namespace ShamansDungeon.ComponentPattern.GUI;


public class InputField : Component
{
    private Collider _col;
    private SpriteRenderer _sr;

    private bool isActive;
    private int caretPosition;
    private float caretBlinkTime; //animation time
    private const float CaretBlinkInterval = 0.5f;
    private StringBuilder text;
    public string Text { get => text.ToString(); }
    private bool hide;

    public InputField(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Awake()
    {
        _col = GameObject.GetComponent<Collider>();
        _sr = GameObject.GetComponent<SpriteRenderer>();
        isActive = false;
        caretPosition = 0;
        caretBlinkTime = 0;
    }

    public override void Update()
    {

    }
}
//    public class InputField : UIComponent
//    {
//        private Rectangle bounds;
//        private SpriteFont font;
//        private Color color;

//        private bool isActive;
//        private int caretPosition;
//        private float caretBlinkTime; //animation time
//        private const float CaretBlinkInterval = 0.5f;
//        private StringBuilder text;
//        public string Text { get => text.ToString(); }
//        private bool hide;

//        public InputField(Rectangle bounds, Color color, bool hide = false)
//        {
//            font = Assets.Fonts[FontName.Clacon2_32];
//            this.color = color;

//            this.bounds = bounds;
//            text = new StringBuilder();
//            isActive = false;
//            caretPosition = 0;
//            caretBlinkTime = 0;

//            this.hide = hide;
//        }

//        public void Update(float deltaTime)
//        {
//            Update(deltaTime, Vector2.Zero);
//        }

//        public void Update(float deltaTime, Vector2 offset)
//        {
//            // Handle mouse input for textbox activation
//            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
//            {
//                var mousePosition = new Point(Mouse.GetState().X, Mouse.GetState().Y);
//                HandleMouseClick(mousePosition, offset);
//            }

//            // Handle writing logic
//            if (isActive)
//            {
//                caretBlinkTime += GameWorld.Instance.DeltaTimeS;
//            }
//        }

//        public void Draw(float deltaTime, SpriteBatch spriteBatch)
//        {
//            Draw(deltaTime, spriteBatch, Vector2.Zero);
//        }

//        public void Draw(float deltaTime, SpriteBatch spriteBatch, Vector2 offset)
//        {
//            //Draw outer border
//            Primitive2D.DrawRectangle(spriteBatch, new Vector2(bounds.X, bounds.Y) + offset, new Vector2(bounds.Width, bounds.Height), Color.White, 2f);

//            var textPosition = new Vector2(bounds.X + 5, bounds.Y + 5) + offset;
//            if (hide)
//            {
//                spriteBatch.DrawString(font, new string('*', text.Length), textPosition, color);
//            }
//            else
//            {
//                spriteBatch.DrawString(font, text, textPosition, color);
//            }

//            if (isActive && (int)(caretBlinkTime / CaretBlinkInterval) % 2 == 0)
//            {
//                var caretPositionInPixels = font.MeasureString(text.ToString().Substring(0, caretPosition));
//                var caret = new Vector2(textPosition.X + caretPositionInPixels.X, textPosition.Y);
//                spriteBatch.DrawString(font, "-", caret, color);
//            }
//        }

//        private void HandleKeyPress(object sender, TextInputEventArgs args)
//        {
//            Keys key = args.Key;
//            char character = args.Character;

//            bool isControlCharacter = Char.IsControl(character); // Control characters are non printable keys like backspace, delete etc.

//            if (isControlCharacter)
//            {
//                if (key == Keys.Back && caretPosition > 0 && text.Length > 0)
//                {
//                    text.Remove(caretPosition - 1, 1);
//                    caretPosition--;
//                }
//                else if (key == Keys.Delete && caretPosition < text.Length)
//                {
//                    text.Remove(caretPosition, 1);
//                }
//                else if (key == Keys.Left && caretPosition > 0)
//                {
//                    caretPosition--;
//                }
//                else if (key == Keys.Right && caretPosition < text.Length)
//                {
//                    caretPosition++;
//                }
//                else if (key == Keys.Enter)
//                {
//                    // Handle Enter key if necessary
//                }
//                return;
//            }

//            if (Char.IsAscii(character))
//            {
//                text.Insert(caretPosition, character);
//                caretPosition++;
//            }
//        }

//        public void HandleMouseClick(Point mousePosition, Vector2 offset)
//        {
//            Rectangle bounds = this.bounds;
//            bounds.Location += offset.ToPoint();

//            if (bounds.Contains(mousePosition))
//            {
//                if (!isActive)
//                {
//                    GameWorld.Instance.Window.TextInput += HandleKeyPress;
//                }
//                isActive = true;
//                caretBlinkTime = 0;
//            }
//            else
//            {
//                if (isActive)
//                {
//                    GameWorld.Instance.Window.TextInput -= HandleKeyPress;
//                }
//                isActive = false;
//            }
//        }

//    }
