using Microsoft.Xna.Framework;

namespace ShamansDungeon.ComponentPattern.Particles.Origins
{
    public class PointOrigin : Origin
    {
        public override OriginData GetPosition(Emitter e)
        {
            return new OriginData(Vector2.Zero);
        }
    }
}
