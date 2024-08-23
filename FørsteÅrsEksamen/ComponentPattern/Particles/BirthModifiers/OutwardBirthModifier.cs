using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public class OutwardBirthModifier : BirthModifier
    {
        public static Vector3 OnSpawnOriginPoint = new(0.1f, 0.1f, 0);

        public override void Execute(Emitter e, GameObject go, IParticle p)
        {
            float v = p.VelocityZ.Length();
            Vector2 temp = p.Position - e.Position; 
            if (temp != Vector2.Zero)
            {
                temp.Normalize();
                p.VelocityZ = new Vector3(temp, 0) * v;
            }
            else
            {
                p.VelocityZ = OnSpawnOriginPoint * v;
            }
        }
    }
}
