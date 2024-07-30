using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorsDungeon.ComponentPattern.Particles.Modifiers
{
    public abstract class Modifier
    {
        public abstract void Execute(Emitter e, double seconds, IParticle p);
        public virtual void OnReset(Emitter e, IParticle p) { }
    }
}
