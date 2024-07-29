using DoctorsDungeon.Other;

namespace DoctorsDungeon.ComponentPattern.Particles.Modifiers
{
    public class GravityModifier : Modifier
    {
        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            p.Velocity += BaseMath.Gravity * (float)seconds;
        }
    }
}
