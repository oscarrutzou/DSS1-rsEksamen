using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShamansDungeon.ComponentPattern.Particles.Modifiers
{
    public class OutwardModifier : Modifier
    {
        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            // Should chech the Emitter point, and go away from it. 
        }
    }
}
