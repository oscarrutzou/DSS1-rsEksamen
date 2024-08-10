using DoctorsDungeon.ComponentPattern.Particles;
using DoctorsDungeon.ComponentPattern;
using DoctorsDungeon;
using DoctorsDungeon.ComponentPattern.Particles.BirthModifiers;
using DoctorsDungeon.ComponentPattern.Particles.Modifiers;
using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.ObjectPoolPattern;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static DoctorsDungeon.ComponentPattern.Particles.Emitter;

namespace DoctorsDungeon.ComponentPattern.Particles
{
    public class ParticleEmitter : Emitter
    {
        private List<GameObject> particleToBeReleased = new();

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
                ReleaseNewParticles();
            }

            TotalSeconds += GameWorld.DeltaTime;
            particleToBeReleased.Clear(); 

            UpdateActiveParticles();

            foreach (GameObject go in particleToBeReleased)
            {
                ParticlePool.ReleaseObject(go);
            }

            if (CanDestroy())
            {
                ParticlePool.ReleaseAllObjects();
            }
        }

        private void ReleaseNewParticles()
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

        private void UpdateActiveParticles()
        {
            double milliseconds = GameWorld.DeltaTime * 1000;
            float dampening = BaseMath.Clamp(1.0f - (float)GameWorld.DeltaTime * LinearDamping, 0.0f, 1.0f);

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

            // CustomDrawingBehavior is if there is a custom way to handle drawing and updating of the ParticleEmitter and particles.
            // Otherwise the particles are going to be deleted when changing scenes, since it would Instantiate in each scene.
            if (CustomDrawingBehavior)
            {
                go.Awake();
                go.Start();
            }
            else
            {
                GameWorld.Instance.Instantiate(go);
            }
        }

        public void EmitParticles(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                AddParticle();
            }
        }

        //public override void Update()
        //{
        //    base.Update();

        //    if (State == EmitterState.RUNNING || State == EmitterState.STOPPING)
        //    {
        //        HandleParticleRelease();
        //    }

        //    UpdateParticleBehavior();
        //    ReleaseExpiredParticles();
        //    CheckForDestruction();
        //}
        ///// <summary>
        ///// Releases particles based on ParticlesPerSecond and MaxParticles
        ///// </summary>
        //private void HandleParticleRelease()
        //{
        //    ReleaseTime += GameWorld.DeltaTime;
        //    double release = ParticlesPerSecond * ReleaseTime;

        //    if (release > 1)
        //    {
        //        int r = (int)Math.Floor(release);
        //        ReleaseTime -= (r / ParticlesPerSecond);

        //        for (int i = 0; i < r; i++)
        //        {
        //            AddParticle();
        //        }
        //    }
        //}

        ///// <summary>
        ///// Updates Age, Position, Rotation and modifiers on each Active Particle
        ///// </summary>
        //private void UpdateParticleBehavior()
        //{
        //    double milliseconds = GameWorld.DeltaTime * 1000;
        //    float dampening = BaseMath.Clamp(1.0f - (float)GameWorld.DeltaTime * LinearDamping, 0.0f, 1.0f);

        //    foreach (GameObject go in ParticlePool.Active)
        //    {
        //        IParticle p = go.GetComponent<Particle>();
        //        p.Age += milliseconds;

        //        // Checks if it should be released since its lifetime is over
        //        if (p.Age > p.MaxAge)
        //        {
        //            particleToBeReleased.Add(go);
        //        }
        //        else
        //        {
        //            go.Transform.Position += (p.Velocity * (float)GameWorld.DeltaTime);
        //            p.Velocity *= dampening; // Optional dampening to slow down the velocity
        //            go.Transform.Rotation += p.RotationVelocity;

        //            // Updates each modifer, to change the data of each particle
        //            foreach (Modifier m in Modifiers)
        //            {
        //                m.Execute(this, GameWorld.DeltaTime, p);
        //            }
        //        }
        //    }
        //}

        //private void ReleaseExpiredParticles()
        //{
        //    foreach (GameObject go in particleToBeReleased)
        //    {
        //        ParticlePool.ReleaseObject(go);
        //    }
        //}

        //private void CheckForDestruction()
        //{
        //    if (CanDestroy())
        //    {
        //        ParticlePool.ReleaseAllObjects();
        //    }
        //}

        ///// <summary>
        ///// Adds a new particle, with relevant start data, and birth modifiers
        ///// </summary>
        //private void AddParticle()
        //{
        //    // OriginData of the current emitter. Changes where each particles spawns.
        //    OriginData originData = Origin.GetPosition(this);
        //    if (originData == null) return;

        //    // Makes a new Particle or uses a previous released particle
        //    GameObject go = ParticlePool.GetObjectAndMake();
        //    if (go == null) return;

        //    // Adds relevant components to the GameObject
        //    SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        //    IParticle particle = go.GetComponent<Particle>();

        //    // Sets the base sprite, so the SpriteRenderer dosent throw an error, by trying to render nothing
        //    sr.Sprite = GlobalTextures.Textures[TextureNames.Pixel4x4]; // If there is no other Textures in the BirthModifiers
        //    sr.ShouldDrawSprite = ShouldShowSprite;

        //    // Sets the start data for each particle
        //    InitializeParticle(particle, go, originData);

        //    // Sets the LayerDepth to be based on, newer particles is in front of older particles.
        //    SetLayerDepth(sr);

        //    // Executes the optional birth modifiers
        //    ExecuteBirthModifiers(go, particle);


        //}

        //private void InitializeParticle(IParticle particle, GameObject go, OriginData originData)
        //{
        //    Matrix matrix = Matrix.CreateRotationZ((float)Direction.GetValue());

        //    particle.Velocity = new Vector2((float)Speed.GetValue(), 0);

        //    particle.Velocity = Vector2.Transform(particle.Velocity, matrix);

        //    particle.Position = Position + originData.Position;

        //    particle.RotationVelocity = (float)RotationVelocity.GetValue();

        //    go.Transform.Rotation = (float)Rotation.GetValue();

        //    particle.MaxAge = MaxAge.GetValue();

        //    particle.Age = 0;

        //    if (TextOnSprite != null)
        //    {
        //        particle.TextOnSprite = (TextOnSprite)TextOnSprite.Clone(); // Sets the new particle to have the same Text
        //    }
        //}

        //private void SetLayerDepth(SpriteRenderer sr)
        //{
        //    double timestamp = DateTime.Now.ToOADate();
        //    double normalizedValue = (timestamp - DateTime.MinValue.ToOADate()) /
        //        (DateTime.MaxValue.ToOADate() - DateTime.MinValue.ToOADate());
        //    double result = -0.001 - normalizedValue * 0.004;
        //    sr.SetLayerDepth(LayerName, Math.Abs((float)result));
        //}

        //private void ExecuteBirthModifiers(GameObject go, IParticle particle)
        //{
        //    foreach (BirthModifier m in BirthModifiers)
        //    {
        //        m.Execute(this, go, particle);
        //    }
        //}

        //public void EmitParticles(int amount = 1)
        //{
        //    for (int i = 0; i < amount; i++)
        //    {
        //        AddParticle();
        //    }
        //}
    }
}

//public override void Update()
//{
//    base.Update();

//    if (State == EmitterState.RUNNING || State == EmitterState.STOPPING)
//    {
//        ReleaseTime += GameWorld.DeltaTime;

//        double release = ParticlesPerSecond * ReleaseTime;
//        if (release > 1)
//        {
//            int r = (int)Math.Floor(release);
//            ReleaseTime -= (r / ParticlesPerSecond);

//            for (int i = 0; i < r; i++)
//            {
//                AddParticle();
//            }

//        }
//    }

//    TotalSeconds += GameWorld.DeltaTime;

//    double milliseconds = GameWorld.DeltaTime * 1000;
//    float dampening = BaseMath.Clamp(1.0f - (float)GameWorld.DeltaTime * LinearDamping, 0.0f, 1.0f);

//    particleToBeReleased.Clear();

//    foreach (GameObject go in ParticlePool.Active)
//    {
//        IParticle p = go.GetComponent<Particle>();

//        p.Age += milliseconds;

//        if (p.Age > p.MaxAge)

//        {
//            //OnParticleDeath(new ParticleEventArgs(p));
//            particleToBeReleased.Add(go);
//        }
//        else
//        {
//            go.Transform.Position += (p.Velocity * (float)GameWorld.DeltaTime);

//            p.Velocity *= dampening;
//            go.Transform.Rotation += p.RotationVelocity;

//            foreach (Modifier m in Modifiers)
//            {
//                m.Execute(this, GameWorld.DeltaTime, p);
//            }
//        }
//    }

//    foreach (GameObject go in particleToBeReleased)
//    {
//        ParticlePool.ReleaseObject(go);
//    }

//    if (CanDestroy())
//    {
//        ParticlePool.ReleaseAllObjects();
//    }
//}

//private void AddParticle()
//{
//    OriginData data = Origin.GetPosition(this);
//    if (data == null) return;

//    GameObject go = ParticlePool.GetObjectAndMake();
//    if (go == null) return;

//    IParticle particle = go.GetComponent<Particle>();
//    SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

//    Matrix matrix = Matrix.CreateRotationZ((float)Direction.GetValue());

//    particle.Velocity = new Vector2((float)Speed.GetValue(), 0);

//    particle.Velocity = Vector2.Transform(particle.Velocity, matrix);

//    particle.Position = Position + data.Position;

//    particle.RotationVelocity = (float)RotationVelocity.GetValue();

//    go.Transform.Rotation = (float)Rotation.GetValue();

//    particle.MaxAge = MaxAge.GetValue();

//    particle.Age = 0;

//    if (TextOnSprite != null)
//    {
//        particle.TextOnSprite = (TextOnSprite)TextOnSprite.Clone(); // Sets the new particle to have the same Text
//    }

//    sr.Sprite = GlobalTextures.Textures[TextureNames.Pixel4x4]; // If there is no other Textures in the BirthModifiers

//    sr.ShouldDrawSprite = ShouldShowSprite;

//    // Should make it so the the offset is always different, and have older paricles under the newer.
//    // Get the current timestamp
//    double timestamp = DateTime.Now.ToOADate();

//    // Normalize the timestamp to [0, 1]
//    double normalizedValue = (timestamp - DateTime.MinValue.ToOADate()) /
//                             (DateTime.MaxValue.ToOADate() - DateTime.MinValue.ToOADate());

//    // Scale the normalized value to [0.001, 0.005]
//    double result = -0.001 - normalizedValue * 0.004;

//    sr.SetLayerDepth(LayerName, Math.Abs((float)result));

//    foreach (BirthModifier m in BirthModifiers) m.Execute(this, go, particle);
//}
//public void EmitParticles(int amount = 1)
//{
//    for (int i = 0; i < amount; i++)
//    {
//        AddParticle();
//    }
//}