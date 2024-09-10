using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShamansDungeon.ComponentPattern.Particles.Modifiers
{
    public class ColorRangeModifier : Modifier
    {
        public ColorInterval ColorInterval;
        private ColorInterval _colorTextInterval;

        private static ColorInterval _rainboxInterval = new ColorInterval(new Color[]
        {
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.Green,
            Color.Blue,
            Color.Violet,
            Color.Transparent,
        });
        private bool _spriteRainbow = false, _textRainbow = false;
        public ColorRangeModifier(Color[] colors, Color[] textColors = null)
        {
            ColorInterval = new ColorInterval(colors);

            if (textColors == null) return;
            _colorTextInterval = new ColorInterval(textColors);
        }

        public ColorRangeModifier(bool spriteRainbow, bool textRainbow = false)
        {
            this._spriteRainbow = spriteRainbow;
            this._textRainbow = textRainbow;
        }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            if (!_spriteRainbow && !_textRainbow)
            {
                p.Color = ColorInterval.GetValue(p.Age / p.MaxAge);

                if (p.TextOnSprite == null || _colorTextInterval == null) return;

                p.TextOnSprite.TextColor = _colorTextInterval.GetValue(p.Age / p.MaxAge);
            }
            else
            {
                p.Color = _rainboxInterval.GetValue(p.Age / p.MaxAge);

                if (_textRainbow == false || p.TextOnSprite == null) return;

                p.TextOnSprite.TextColor = _rainboxInterval.GetValue(p.Age / p.MaxAge);
            }
        }
    }
}
