using ShamansDungeon.ComponentPattern.Particles;
using ShamansDungeon.ComponentPattern;
using ShamansDungeon;
using ShamansDungeon.ComponentPattern.Particles.BirthModifiers;
using ShamansDungeon.ComponentPattern.Particles.Modifiers;
using ShamansDungeon.ComponentPattern.Particles.Origins;
using ShamansDungeon.ComponentPattern.PlayerClasses;
using ShamansDungeon.ComponentPattern.WorldObjects;
using ShamansDungeon.GameManagement;
using ShamansDungeon.ObjectPoolPattern;
using ShamansDungeon.Other;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ShamansDungeon.ComponentPattern.Particles;

public class ParticleEmitter : Emitter
{
    private List<GameObject> _particleToBeReleased = new();

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
        _particleToBeReleased.Clear(); 

        UpdateActiveParticles();

        foreach (GameObject go in _particleToBeReleased)
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
                _particleToBeReleased.Add(go);
            }
            else
            {
                //go.Transform.Position += (p.Velocity * (float)GameWorld.DeltaTime);
                if (p.VelocityZ.Z < 0)
                {

                }
                p.PositionZ += (p.VelocityZ * (float)GameWorld.DeltaTime);
                p.VelocityZ *= dampening; // If we a modifier to be able to change velocity, we need to have 0 dampening

                go.Transform.Rotation += p.RotationVelocity;

                foreach (Modifier m in Modifiers.Values)
                {
                    m.Execute(this, GameWorld.DeltaTime, p);
                }
            }
        }
    }
    public int StartZVel = 10;
    private Interval direction = new Interval(0, -MathHelper.Pi);
    private void AddParticle()
    {
        OriginData data = Origin.GetPosition(this);
        if (data == null) return;

        GameObject go = ParticlePool.GetObjectAndMake();
        if (go == null) return;

        IParticle particle = go.GetComponent<Particle>();
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        
        Matrix matrix = Matrix.CreateRotationZ((float)Direction.GetValue());
        Vector3 rotatedVel = Vector3.Transform(new Vector3((float)SpeedX.GetValue(), 0, StartZVel), matrix);
        particle.VelocityZ = rotatedVel;

        // Use the desiredSpeedY to set the VelocityZ
        //Vector3 velocity = new Vector3((float)SpeedX.GetValue(), (float)SpeedX.GetValue(), StartZVel);
        //Vector3 rotatedVel = Vector3.Transform(velocity, matrix);


        particle.Position = Position + data.Position;

        particle.PositionZ = new Vector3(particle.Position, 0);

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

        foreach (BirthModifier m in BirthModifiers.Values) m.Execute(this, go, particle);

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
}
