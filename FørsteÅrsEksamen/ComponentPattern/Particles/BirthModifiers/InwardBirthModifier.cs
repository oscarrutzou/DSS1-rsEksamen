using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public class InwardBirthModifier : BirthModifier
    {
        public override void Execute(Emitter e, GameObject go, IParticle p)
        {
            float v = p.Velocity.Length();
            Vector2 temp = e.Position - p.Position;
            if (temp != Vector2.Zero)
            {
                temp.Normalize();
                p.Velocity = temp * v;
            }
            else
            {
                p.Velocity = OutwardBirthModifier.OnSpawnOriginPoint * v;
            }
        }
    }
}
