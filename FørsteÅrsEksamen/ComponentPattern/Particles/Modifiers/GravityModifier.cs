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

            //double normalizeAge = p.Age / p.MaxAge;
            //if (normalizeAge < 0.2f)
            //{
            //    p.VelocityZ += gravity;
            //}
            //else
            //{
            //    if (p.VelocityZ.X > 0) p.VelocityZ -= new Vector3(gravity.X, 0, 0);
            //    else p.VelocityZ = new Vector3(0, gravity.Y, gravity.Z);

            //    if (p.VelocityZ.Y > 0) p.VelocityZ -= new Vector3(0, gravity.Y, 0);
            //    else p.VelocityZ = new Vector3(gravity.X, 0, gravity.Z);

            //    if (p.VelocityZ.Z > 0) p.VelocityZ -= new Vector3(0, 0, gravity.Z);
            //    else p.VelocityZ = new Vector3(gravity.X, gravity.Y, 0);
            //}

            //if (p.VelocityZ.Z >= 1f)
            //{
            //    p.VelocityZ = Vector3.Zero;
            //}
            //float add = 1f * (float)GameWorld.DeltaTime;
            //p.PositionZ = new Vector3(go.Transform.Position.X, go.Transform.Position.Y, p.PositionZ.Z + add);

            //float test = dampening;
            //if (p.PositionZ.Z > 2f)
            //{
            //    test = BaseMath.Clamp(1.0f - (float)GameWorld.DeltaTime * (float)p.Age * 0.0005f, 0.0f, 1.0f);
            //}


            // Start VelZ = 10 for alle
            // Start posZ = 5 for alle

            // 
            // ForceZ = 100
            // Deltatime = 0.01667
            // drag = 0.95

            /*bounce 0.05f, draw 0.025f
             * Force = Velocity
             *  p_PosZ[i] +=  p_VelZ[i] * deltaTime;
                if (p_PosZ[i] < 0 && colidesWith0ZPlane)
                {
                    p_PosZ[i] = 0;
                    p_VelZ[i] *= -bounce;

                
                p_VelZ[i] += (p_ForceZ[i] * deltaTime) - (p_VelZ[i] * drag * deltaTime);
             */
            p.VelocityZ += new Vector3(Gravity, 0) * (float)seconds;

        }
    }
}
