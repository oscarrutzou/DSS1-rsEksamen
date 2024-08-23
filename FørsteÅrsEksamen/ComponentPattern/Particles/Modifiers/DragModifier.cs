
using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.Particles.Modifiers
{
    public class DragModifier : Modifier
    {
        private float _bounce = 0.25f;
        private float _friction = 0.85f;
        private int _stopAmount = 60;
        private float _velocityDraw = 9.75f;

        public DragModifier(float bounce, float friction, int stopAmount, float velocityDraw)
        {
            UpdateValue(bounce, friction, stopAmount, velocityDraw);
        }

        public void UpdateValue(float bounce, float friction, int stopAmount, float velocityDraw)
        {
            this._bounce = bounce;
            this._friction = friction;
            this._stopAmount = stopAmount;
            this._velocityDraw = velocityDraw;
        }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            if (p.PositionZ.Z < 0)
            {
                p.PositionZ = new Vector3(p.PositionZ.X, p.PositionZ.Y, 0);
                // Times with fiction and bounce
                p.VelocityZ = new Vector3(p.VelocityZ.X * _friction, p.VelocityZ.Y * _friction, p.VelocityZ.Y * -_bounce);
            }

            float zAmount = (float)((-_stopAmount * seconds) - (p.VelocityZ.Z * _velocityDraw * seconds));
            p.VelocityZ += new Vector3(0, 0, zAmount);        
        }
    }
}
