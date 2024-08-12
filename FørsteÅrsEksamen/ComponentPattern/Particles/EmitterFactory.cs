

using DoctorsDungeon.ComponentPattern.Particles.BirthModifiers;
using DoctorsDungeon.ComponentPattern.Particles.Modifiers;
using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.GameManagement;
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

        public static GameObject TextDamageEmitter(Color[] colors, GameObject followObj, Vector2 followOffset, Origin emitterOrigin = null)
        {
            GameObject go = CreateParticleEmitter("Text Damage Taken", new Vector2(200, -200), new Interval(50, 100), new Interval(-MathHelper.Pi, 0), 0, new Interval(400, 600), 100, -1, new Interval(-0.5f, 0.5f), new Interval(-0.001f, 0.001f));

            ParticleEmitter emitter = go.GetComponent<ParticleEmitter>();
            emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));
            emitter.AddModifier(new ColorRangeModifier(new Color[] { Color.Transparent }, colors));
            emitter.AddModifier(new ScaleModifier(2, 1));
            emitter.FollowGameObject(followObj, followOffset);

            if (emitterOrigin != null)
                emitter.Origin = emitterOrigin;

            return go;
        }
    }
}
