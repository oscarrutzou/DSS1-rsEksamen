using DoctorsDungeon.ComponentPattern.Particles.BirthModifiers;
using DoctorsDungeon.ComponentPattern.Particles.Modifiers;
using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Particles
{
    public class ParticleEmitter : Emitter
    {

        List<GameObject> particleToBeReleased = new();

        public ParticleEmitter(GameObject gameObject) : base(gameObject)
        {
        }

        public ParticleEmitter(GameObject gameObject, string name, Vector2 pos, Interval speed, Interval direction, float particlesPerSecond, Interval maxAge, int maxAmount) : base(gameObject, name, pos, speed, direction, particlesPerSecond, maxAge, maxAmount)
        {
        }

        public override void Update()
        {
            base.Update();

            if (State == EmitterState.RUNNING || State == EmitterState.STOPPING)
            {
                ReleaseTime += GameWorld.DeltaTime;

                double release = ParticlesPerSecond * ReleaseTime;
                if (release > 1)
                {
                    int r = (int)Math.Floor(release);
                    ReleaseTime -= (r / ParticlesPerSecond);

                    for (int i = 0; i < r; i++)
                    {
                        AddParticle();
                    }
                }
            }

            TotalSeconds += GameWorld.DeltaTime;
            double milliseconds = GameWorld.DeltaTime * 1000;
            float dampening = BaseMath.Clamp(1.0f - (float)GameWorld.DeltaTime * LinearDamping, 0.0f, 1.0f);

            particleToBeReleased.Clear();

            foreach (GameObject go in ParticlePool.Active)
            {
                IParticle p = go.GetComponent<Particle>();

                p.Age += milliseconds;

                if (p.Age > p.MaxAge)
                {
                    //OnParticleDeath(new ParticleEventArgs(p));
                    particleToBeReleased.Add(go);
                }
                else
                {
                    go.Transform.Position += (p.Velocity * (float)GameWorld.DeltaTime);

                    p.Velocity *= dampening;
                    go.Transform.Rotation += p.RotationVelocity;

                    foreach (Modifier m in Modifiers)
                    {
                        m.Execute(this, GameWorld.DeltaTime, p);
                    }
                }
            }

            foreach (GameObject go in particleToBeReleased)
            {
                ParticlePool.ReleaseObject(go);
            }

            if (CanDestroy())
            {
                ParticlePool.ReleaseAllObjects();
                GameWorld.Instance.Destroy(GameObject);
            }
        }

        public void AddParticle()
        {
            OriginData data = Origin.GetPosition(this);
            if (data == null) return;

            GameObject go = ParticlePool.GetObjectAndMake();

            if (go == null) return;

            IParticle particle = go.GetComponent<Particle>();
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

            Matrix matrix = Matrix.CreateRotationZ((float)Direction.GetValue());

            particle.Position = Position;

            particle.Velocity = new Vector2((float)Speed.GetValue(), 0);
            particle.Velocity = Vector2.Transform(particle.Velocity, matrix);
            particle.Position = Position + data.Position;
            if (Origin.UseColorData) particle.Color = data.Color;
            particle.RotationVelocity = (float)RotationVelocity.GetValue();
            go.Transform.Rotation = (float)Rotation.GetValue();
            particle.MaxAge = MaxAge.GetValue();
            particle.Age = 0;
            sr.Sprite = GlobalTextures.Textures[TextureNames.Pixel];

            foreach (BirthModifier m in BirthModifiers) m.Execute(this, go, particle);
        }
    }
}
