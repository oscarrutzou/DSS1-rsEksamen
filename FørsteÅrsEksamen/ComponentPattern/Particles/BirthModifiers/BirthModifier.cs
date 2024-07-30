

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public abstract class BirthModifier
    {
        public abstract void Execute(Emitter e, GameObject go, IParticle p);
        public virtual void OnReset(Emitter e, GameObject go, IParticle p) { }
    }
}
