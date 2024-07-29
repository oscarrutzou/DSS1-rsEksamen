using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public class OutwardBirthModifier : BirthModifier
    {
        public override void Execute(Emitter e, GameObject go, IParticle p)
        {
            float v = p.Velocity.Length();
            Vector2 temp = p.Position - e.Position;
            temp.Normalize();
            p.Velocity = temp * v;
        }
    }
}
