using System;
using Microsoft.Xna.Framework;

namespace ShamansDungeon.ComponentPattern.Particles.Origins
{
    public class CircleOrigin : Origin
    {
        private readonly Interval _dist;
        private readonly Interval _angle = new Interval(-Math.PI, Math.PI);
        private readonly bool _edge;
        private readonly int _radius;

        public CircleOrigin(int radius, bool edge = false)
        {
            _dist = new Interval(0, radius);
            _edge = edge;
            _radius = radius;
        }

        public override OriginData GetPosition(Emitter e)
        {
            Matrix rotation = Matrix.CreateRotationZ((float)_angle.GetValue());
            if (_edge)
            {
                Vector2 p = new Vector2(_radius, 0);
                return new OriginData(Vector2.Transform(p, rotation));
            }
            else
            {
                Vector2 p = new Vector2((int)_dist.GetValue(), 0);
                return new OriginData(Vector2.Transform(p, rotation));
            }
        }
    }
}
