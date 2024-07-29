

using DoctorsDungeon.ComponentPattern.Particles.BirthModifiers;
using DoctorsDungeon.ComponentPattern.Particles.Modifiers;
using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ObjectPoolPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Particles
{
    public class Emitter : Component
    {
        public enum EmitterState { INIT, RUNNING, STOPPING, STOPPED }

        public Texture2D Texture { get; set; }
        public Vector2 Position
        {
            get { return GameObject.Transform.Position; }
            set { GameObject.Transform.Position = value; }
        }
        public string Name { get; set; }
        public float ParticlesPerSecond;
        public float LinearDamping { get; set; }
        public Origin Origin { get; set; } = new PointOrigin();

        public double TotalSeconds { get; set; }
        protected Interval Speed;

        protected List<Modifier> Modifiers { get; set; } = new();
        protected List<BirthModifier> BirthModifiers { get; set; } = new();
        protected Interval MaxAge;
        protected EmitterState State = EmitterState.INIT;
        protected double ReleaseTime = 0;
        protected Interval Direction;
        protected Interval Rotation = new Interval(-Math.PI, Math.PI);
        protected Interval RotationVelocity = new Interval(-0.1f, 0.1f);

        private double _stopTime;
        private float _stopCount;

        public ParticlePool ParticlePool { get; set; } = new();

        public Emitter(GameObject gameObject) : base(gameObject)
        {
        }

        public Emitter(GameObject gameObject, string name, Vector2 pos, Interval speed, Interval direction, float particlesPerSecond, Interval maxAge, int maxAmount, Interval rotationVelocity = null) : base(gameObject)
        {
            Name = name;
            Position = pos;
            Speed = speed;
            Direction = direction;
            ParticlesPerSecond = particlesPerSecond;
            MaxAge = maxAge;
            ParticlePool.MaxAmount = maxAmount;

            if (rotationVelocity != null)
            {
                RotationVelocity = rotationVelocity;
            }
            else
            {
                RotationVelocity = new Interval(-0.1f, 0.1f);
            }
        }

        public virtual void AddModifier(Modifier modifier)
        {
            Modifiers.Add(modifier);
        }

        public virtual void AddBirthModifier(BirthModifier modifier)
        {
            BirthModifiers.Add(modifier);
        }

        public void StartEmitter()
        {
            ReleaseTime = 0;
            State = EmitterState.RUNNING;
        }

        public void StopEmitter()
        {
            if (State == EmitterState.RUNNING)
            {
                State = EmitterState.STOPPING;
                _stopCount = ParticlesPerSecond;
            }
        }

        public bool CanDestroy()
        {
            return ParticlePool.Active.Count == 0 && State == EmitterState.STOPPED;
        }

        public override void Update()
        {
            if (State == EmitterState.STOPPING)
            {
                _stopTime += GameWorld.DeltaTime;
                ParticlesPerSecond = MathHelper.SmoothStep(_stopCount, 0, (float)_stopTime);
                if (ParticlesPerSecond <= 0)
                {
                    State = EmitterState.STOPPED;
                }
            }
        }

    }
}
