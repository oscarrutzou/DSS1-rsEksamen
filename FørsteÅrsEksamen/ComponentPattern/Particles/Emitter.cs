

using DoctorsDungeon.ComponentPattern.Particles.BirthModifiers;
using DoctorsDungeon.ComponentPattern.Particles.Modifiers;
using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ObjectPoolPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Particles;

public class Emitter : Component
{
    #region Properties
    public enum EmitterState { INIT, RUNNING, STOPPING, STOPPED }
    public Texture2D Texture { get; set; }
    public Vector2 Position
    {
        get { return GameObject.Transform.Position; }
        set { GameObject.Transform.Position = value; }
    }
    public Vector2 FollowPoint { get; set; }
    public LayerDepth LayerName { get; set; }
    protected GameObject FollowObject { get; private set; }
    protected Vector2 FollowObjectOffset { get; private set; }
    public string EmitterName { get; set; }
    public float MaxParticlesPerSecond { get; set; }
    public float ParticlesPerSecond { get; set; }
    public float LinearDamping { get; set; }
    public Origin Origin { get; set; } = new PointOrigin();
    protected bool ShouldShowSprite { get; set; } = true;
    public double TotalSeconds { get; set; }
    protected Interval Speed;

    public Dictionary<Type, Modifier> Modifiers { get; set; } = new();
    public Dictionary<Type, BirthModifier> BirthModifiers { get; set; } = new();
    protected Interval MaxAge;
    public EmitterState State = EmitterState.INIT;
    protected double ReleaseTime { get; set; } = 0;
    protected Interval Direction;
    protected Interval Rotation;
    protected Interval RotationVelocity;
    protected TextOnSprite TextOnSprite;

    private double _stopTime;
    private float _stopCount;

    private readonly double timeBeforeStopping;
    public double Timer;

    public ParticlePool ParticlePool { get; set; } = new();

    public bool CustomDrawingBehavior { get; set; } = false;
    #endregion

    public Emitter(GameObject gameObject) : base(gameObject)
    {
    }

    /// <summary>
    /// A emitter that uses particles, modifiers and origins to change animations
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="name"></param>
    /// <param name="pos"></param>
    /// <param name="speed"></param>
    /// <param name="direction"></param>
    /// <param name="particlesPerSecond"></param>
    /// <param name="maxAge"></param>
    /// <param name="maxAmount"></param>
    /// <param name="timeBeforeStop">If the time is not set, the emitter will not stop</param>
    /// <param name="rotationVelocity"></param>
    public Emitter(GameObject gameObject, string name, Vector2 pos, Interval speed, Interval direction, float particlesPerSecond, Interval maxAge, int maxAmount, double timeBeforeStop = -1, Interval rotation = null, Interval rotationVelocity = null) : base(gameObject)
    {
        EmitterName = name;
        Position = pos;
        Speed = speed;
        Direction = direction;
        MaxParticlesPerSecond = particlesPerSecond;
        ParticlesPerSecond = MaxParticlesPerSecond;
        MaxAge = maxAge;
        ParticlePool.MaxAmount = maxAmount;

        timeBeforeStopping = timeBeforeStop;

        if (rotation != null)
        {
            Rotation = rotation;
        }
        else
        {
            Rotation = new(-Math.PI, Math.PI);
        }

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
        Type type = modifier.GetType();
        Modifiers.Add(type, modifier);
    }

    public virtual void AddBirthModifier(BirthModifier modifier)
    {
        Type type = modifier.GetType();
        BirthModifiers.Add(type, modifier);
    }

    public T GetModifier<T>() where T : Modifier
    {
        Type modifierType = typeof(T);

        // Make a check to see if "T" is a subclass 
        foreach (Modifier modifierVal in Modifiers.Values)
        {
            if (modifierType.IsAssignableFrom(modifierVal.GetType()))
            {
                return (T)modifierVal;
            }
        }
        // Cant find the class T
        return null;
    }

    public T GetBirthModifier<T>() where T : BirthModifier
    {
        Type modifierType = typeof(T);

        // Make a check to see if "T" is a subclass 
        foreach (BirthModifier modifierVal in BirthModifiers.Values)
        {
            if (modifierType.IsAssignableFrom(modifierVal.GetType()))
            {
                return (T)modifierVal;
            }
        }
        // Cant find the class T
        return null;
    }

    public void FollowGameObject(GameObject gameObject, Vector2 offset)
    {
        FollowObject = gameObject;
        FollowObjectOffset = offset;
    }

    public void ResetFollowGameObject()
    {
        FollowObject = null;
        FollowObjectOffset = Vector2.Zero;
        Position = Vector2.Zero;
    }

    public void SetParticleText(TextOnSprite textOnSprite, bool showSprite = false)
    {
        TextOnSprite = textOnSprite;
        ShouldShowSprite = showSprite;
    }

    /// <summary>
    /// Starts the normal emitter
    /// </summary>
    public void StartEmitter()
    {
        ReleaseTime = 0;
        ParticlesPerSecond = MaxParticlesPerSecond;
        State = EmitterState.RUNNING;
        Timer = 0; // Reset timer to be able to start and stop the emitter again
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
        if (FollowObject != null)
        {
            Position = FollowObject.Transform.Position + FollowObjectOffset;
        }

        if (State == EmitterState.RUNNING && timeBeforeStopping != -1)
        {
            Timer += GameWorld.DeltaTime;

            if (Timer < timeBeforeStopping) return; // Dont do anything if the timer is not there yet

            StopEmitter();
        }

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
