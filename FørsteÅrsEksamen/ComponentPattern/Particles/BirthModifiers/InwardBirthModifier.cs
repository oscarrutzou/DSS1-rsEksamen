using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public class InwardBirthModifier : BirthModifier
    {
        public override void Execute(Emitter e, GameObject go, IParticle p)
        {
            float v = p.VelocityZ.Length();
            Vector2 targetPos;
            if (e.FollowPoint != Vector2.Zero) targetPos = e.FollowPoint;
            else targetPos = e.Position;

            Vector2 temp = targetPos - p.Position;
            if (temp != Vector2.Zero)
            {
                temp.Normalize();
                p.VelocityZ = new Vector3(temp, 0) * v;
            }
            else
            {
                p.VelocityZ = OutwardBirthModifier.OnSpawnOriginPoint * v;
            }
        }
    }
}
