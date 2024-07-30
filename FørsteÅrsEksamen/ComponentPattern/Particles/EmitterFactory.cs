

using Microsoft.Xna.Framework;

namespace DoctorsDungeon.ComponentPattern.Particles
{
    public static class EmitterFactory
    {
        public static GameObject CreateParticleEmitter(string name, Vector2 pos, Interval speed, Interval direction, float particlesPerSecond, Interval maxAge, int maxAmount, double timeBeforeStop = -1, Interval rotation = null, Interval rotationVelocity = null)
        {
            GameObject go = new();
            go.Type = GameObjectTypes.Emitter;
            
            go.AddComponent<ParticleEmitter>(name, pos, speed, direction, particlesPerSecond, maxAge, maxAmount, timeBeforeStop, rotation, rotationVelocity);

            return go;
        }
    }
}
