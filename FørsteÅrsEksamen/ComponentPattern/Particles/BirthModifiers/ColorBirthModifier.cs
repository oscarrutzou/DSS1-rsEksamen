using Microsoft.Xna.Framework;
using System;

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public class ColorBirthModifier : BirthModifier
    {
        private ColorInterval _colorInterval;

        public ColorBirthModifier(params Color[] colors)
        {
            _colorInterval = new ColorInterval(colors);
        }
        public override void Execute(Emitter e, GameObject go, IParticle p)
        {
            p.Color = _colorInterval.GetValue(e.TotalSeconds - Math.Truncate(e.TotalSeconds));
        }
    }
}
