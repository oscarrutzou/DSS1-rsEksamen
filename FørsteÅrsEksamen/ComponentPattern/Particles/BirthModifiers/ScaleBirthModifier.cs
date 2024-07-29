using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public class ScaleBirthModifier : BirthModifier
    {
        private readonly Interval _interval;

        public ScaleBirthModifier(Interval scales)
        {
            _interval = scales;
        }

        public override void Execute(Emitter e, GameObject go, IParticle p)
        {
            float scale = (float)_interval.GetValue();
            p.Scale = new Vector2(scale, scale);
        }
    }
}
