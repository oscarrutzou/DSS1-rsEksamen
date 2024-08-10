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

        public ParticleEmitter(GameObject gameObject, string name, Vector2 pos, Interval speed, Interval direction, float particlesPerSecond, Interval maxAge, int maxAmount, double timeBeforeStop = -1, Interval rotation = null, Interval rotationVelocity = null) : base(gameObject, name, pos, speed, direction, particlesPerSecond, maxAge, maxAmount, timeBeforeStop, rotation, rotationVelocity)
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
            }
        }

        private void AddParticle()
        {
            OriginData data = Origin.GetPosition(this);
            if (data == null) return;

            GameObject go = ParticlePool.GetObjectAndMake();
            if (go == null) return;

            IParticle particle = go.GetComponent<Particle>();
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

            Matrix matrix = Matrix.CreateRotationZ((float)Direction.GetValue());

            particle.Velocity = new Vector2((float)Speed.GetValue(), 0);

            particle.Velocity = Vector2.Transform(particle.Velocity, matrix);

            particle.Position = Position + data.Position;

            particle.RotationVelocity = (float)RotationVelocity.GetValue();
            
            go.Transform.Rotation = (float)Rotation.GetValue();
            
            particle.MaxAge = MaxAge.GetValue();
            
            particle.Age = 0;

            if (TextOnSprite != null)
            {
                particle.TextOnSprite = (TextOnSprite)TextOnSprite.Clone(); // Sets the new particle to have the same Text
            }

            sr.Sprite = GlobalTextures.Textures[TextureNames.Pixel4x4]; // If there is no other Textures in the BirthModifiers

            sr.ShouldDrawSprite = ShouldShowSprite;

            // Should make it so the the offset is always different, and have older paricles under the newer.
            // Get the current timestamp
            double timestamp = DateTime.Now.ToOADate();

            // Normalize the timestamp to [0, 1]
            double normalizedValue = (timestamp - DateTime.MinValue.ToOADate()) /
                                     (DateTime.MaxValue.ToOADate() - DateTime.MinValue.ToOADate());

            // Scale the normalized value to [0.001, 0.005]
            double result = -0.001 - normalizedValue * 0.004;

            sr.SetLayerDepth(LayerName, Math.Abs((float)result));

            foreach (BirthModifier m in BirthModifiers) m.Execute(this, go, particle);
        }


        public void EmitParticles(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                AddParticle();
            }
        }
    }
}
