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

            if (temp == Vector2.Zero) return;
            temp.Normalize();

            float z = 0;
            if (p.VelocityZ != Vector3.Zero)
            {
                Vector3 normalizedVel = Vector3.Normalize(p.VelocityZ);
                z = normalizedVel.Z;
            }
            p.VelocityZ = new Vector3(temp, z) * v;
        }
    }
}
