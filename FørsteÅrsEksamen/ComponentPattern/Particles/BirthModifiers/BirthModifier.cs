

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public abstract class BirthModifier
    {
        public abstract void Execute(Emitter e, GameObject go, IParticle p);
    }
}
