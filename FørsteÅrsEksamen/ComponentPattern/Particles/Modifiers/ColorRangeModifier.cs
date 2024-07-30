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

        public ColorRangeModifier(Color[] colors, Color[] textColors = null)
        {
            _colorInterval = new ColorInterval(colors);

            if (textColors == null) return;
            _colorTextInterval = new ColorInterval(textColors);
        }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            p.Color = _colorInterval.GetValue(p.Age / p.MaxAge);
            
            if (p.TextOnSprite == null || _colorTextInterval == null) return;
            
            p.TextOnSprite.TextColor = _colorTextInterval.GetValue(p.Age / p.MaxAge);
        }
    }
}
