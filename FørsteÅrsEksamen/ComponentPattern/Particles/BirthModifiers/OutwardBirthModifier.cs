using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public class OutwardBirthModifier : BirthModifier
    {
        public static Vector2 OnSpawnOriginPoint = new(0.4f, 0.3f);
        // Have the problem if the temp is zero, the velocity is also zero?
        public override void Execute(Emitter e, GameObject go, IParticle p)
        {
            float v = p.Velocity.Length();
            Vector2 temp = p.Position - e.Position;
            if (temp != Vector2.Zero)
            {
                temp.Normalize();
                p.Velocity = temp * v;
            }
            else
            {
                p.Velocity = OnSpawnOriginPoint * v;
            }
        }
    }
}
