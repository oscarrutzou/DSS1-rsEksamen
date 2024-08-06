using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.Particles.Modifiers
{
    public class ColorRangeModifier : Modifier
    {
        private ColorInterval _colorInterval;
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
        private bool spriteRainbow = false, textRainbow = false;
        public ColorRangeModifier(Color[] colors, Color[] textColors = null)
        {
            _colorInterval = new ColorInterval(colors);

            if (textColors == null) return;
            _colorTextInterval = new ColorInterval(textColors);
        }

        public ColorRangeModifier(bool spriteRainbow, bool textRainbow = false)
        {
            this.spriteRainbow = spriteRainbow;
            this.textRainbow = textRainbow;
        }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            if (!spriteRainbow && !textRainbow)
            {
                p.Color = _colorInterval.GetValue(p.Age / p.MaxAge);

                if (p.TextOnSprite == null || _colorTextInterval == null) return;

                p.TextOnSprite.TextColor = _colorTextInterval.GetValue(p.Age / p.MaxAge);
            }
            else
            {
                p.Color = _rainboxInterval.GetValue(p.Age / p.MaxAge);

                if (textRainbow == false || p.TextOnSprite == null) return;

                p.TextOnSprite.TextColor = _rainboxInterval.GetValue(p.Age / p.MaxAge);
            }
        }
    }
}
