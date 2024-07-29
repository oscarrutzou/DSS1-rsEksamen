using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public class InwardBirthModifier : BirthModifier
    {
        public override void Execute(Emitter e, GameObject go, IParticle p)
        {
            float v = p.Velocity.Length();
            Vector2 temp = e.Position - p.Position;
            temp.Normalize();
            p.Velocity = temp * v;
        }
    }
}
