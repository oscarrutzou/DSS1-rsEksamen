using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.Particles.Modifiers
{
    public class ScaleModifier : Modifier
    {
        private float _start;
        private float _end;

        /// <summary>
        /// -1 for the start means that it starts from its base scale
        /// </summary>
        /// <param name="end"></param>
        /// <param name="start"></param>
        public ScaleModifier(float end, float start = -1)
        {
            _start = start;
            _end = end;
        }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            if (_start == -1)
            {
                _start = p.Scale.X;
            }

            float scale = MathHelper.Lerp(_start, _end, (float)(p.Age / p.MaxAge));
            p.Scale = new Vector2(scale, scale);
        }
    }
}
