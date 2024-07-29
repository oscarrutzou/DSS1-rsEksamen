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

        public ColorRangeModifier(params Color[] colors)
        {
            _colorInterval = new ColorInterval(colors);
        }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            p.Color = _colorInterval.GetValue(p.Age / p.MaxAge);
        }
    }
}
