using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.Particles.Modifiers
{
    public class GravityModifier : Modifier
    {
        public float GravityScale = 20.0f;
        public Vector2 Gravity { get; set; }

        public GravityModifier(float gravityScale = 20.0f)
        {
            Gravity = new Vector2(0, 10.0f * gravityScale);
        }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            p.VelocityZ += new Vector3(Gravity, 0) * (float)seconds;
        }
    }
}
