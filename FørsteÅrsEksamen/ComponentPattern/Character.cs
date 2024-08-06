using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern.Particles.BirthModifiers;
using DoctorsDungeon.ComponentPattern.Particles.Modifiers;
using DoctorsDungeon.ComponentPattern.Particles.Origins;
using DoctorsDungeon.ComponentPattern.Particles;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.Weapons;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace DoctorsDungeon.ComponentPattern;

public enum AnimationDirectionState
{
    Left,
    Right
}

public enum CharacterState
{
    Idle,
    Moving,
    Attacking,
    Dead
}

// Oscar
public abstract class Character : Component
{
    #region Properties

    public GameObject WeaponGo, HandLeft, HandRight;

    protected SpriteRenderer SpriteRenderer;
    protected Animator Animator;
    protected Collider Collider;
    protected Weapon Weapon;
    protected Health Health;

    protected Dictionary<CharacterState, AnimNames> CharacterStateAnimations = new();
    public static Vector2 SmallSpriteOffset = new(0, -32); // Move the animation up a bit so it looks like it walks correctly.
    public static Vector2 LargeSpriteOffSet = new(0, -96); // Move the animation up more since its a 64x64 insted of 32x32 canvans, for the Run and Death.

    public CharacterState State { get; protected set; } = CharacterState.Moving; // We use the method SetState, to we can change the animations and other variables.
    public Vector2 Direction { get; protected set; }
    protected AnimationDirectionState DirectionState = AnimationDirectionState.Right;

    protected float AttackTimer;
    protected float AttackCooldown = 2f;

    protected int Speed { get; set; }
    public int CollisionNr { get; set; }

    #endregion Properties

    public Character(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Awake()
    {
        SpriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        Animator = GameObject.GetComponent<Animator>();
        Collider = GameObject.GetComponent<Collider>();

        Health = GameObject.GetComponent<Health>();
        SetActionInHealth();

        if (WeaponGo != null)
        {
            Weapon = WeaponGo.GetComponent<Weapon>();
            Weapon.MoveWeaponAndAngle();
        }

        MakeEmitters();
    }

    private void SetActionInHealth()
    {
        Health.OnDamageTaken += OnDamageTaken;
        Health.OnZeroHealth += OnDie;
        Health.OnResetColor += OnResetColor;
    }

    // This is not a abstract method since we only need to set it in the Player and Enemy class, and not in its subclasses
    /// <summary>
    /// A method to set the new state and change the animation drawn.
    /// </summary>
    /// <param name="newState"></param>
    protected virtual void SetState(CharacterState newState)
    {
        if (State == newState) return; // Dont change the state to the same and reset the animation
        State = newState;

        //if (State == CharacterState.Moving)
        //{
        //    emitterDustCloud.StartEmitter();
        //}
        //else
        //{
        //    emitterDustCloud.StopEmitter();
        //}

        switch (State)
        {
            case CharacterState.Idle:
                Animator.PlayAnimation(CharacterStateAnimations[State]);

                SpriteRenderer.OriginOffSet = SmallSpriteOffset;
                break;

            case CharacterState.Moving:
                Animator.PlayAnimation(CharacterStateAnimations[State]);

                SpriteRenderer.OriginOffSet = LargeSpriteOffSet;
                break;

            case CharacterState.Attacking:
                Animator.PlayAnimation(CharacterStateAnimations[CharacterState.Idle]); // Just uses the Idle since we have no attacking animation

                SpriteRenderer.OriginOffSet = SmallSpriteOffset;
                break;

            case CharacterState.Dead:
                Animator.PlayAnimation(CharacterStateAnimations[State]);

                SpriteRenderer.OriginOffSet = LargeSpriteOffSet;
                Animator.StopCurrentAnimationAtLastSprite();
                break;
        }
    }

    /// <summary>
    /// Updates the direction of which way the sprite should draw. Remember to set the direction!
    /// </summary>
    protected virtual void UpdateDirection()
    {
        if (Direction.X >= 0)
        {
            DirectionState = AnimationDirectionState.Right;
            SpriteRenderer.SpriteEffects = SpriteEffects.None;
        }
        else if (Direction.X < 0)
        {
            DirectionState = AnimationDirectionState.Left;
            SpriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
        }
    }

    public void Attack()
    {
        if (Weapon == null) return;
        Weapon.MoveWeaponAndAngle(); // Should maybe wait till it has reached towards the new direction
        if (Weapon == null) return;
        Weapon.StartAttack();
    }

    private void OnDie()
    {
        SetState(CharacterState.Dead);
        GameWorld.Instance.Destroy(WeaponGo);
        Weapon = null;

        // Remove hands
        SpriteRenderer.Color = Color.LightPink;

        Health.OnDamageTaken -= OnDamageTaken;
        Health.OnZeroHealth -= OnDie;
        Health.OnResetColor -= OnResetColor;
    }

    private void OnDamageTaken()
    {
        Weapon.SpriteRenderer.Color = Health.DamageTakenColor;
    }

    private void OnResetColor()
    {
        Weapon.SpriteRenderer.Color = Color.White;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        //damageTakenEmitter.LayerName = SpriteRenderer.LayerName;

        if (!InputHandler.Instance.DebugMode) return;
        Vector2 center = GameObject.Transform.Position - new Vector2(5, 5);
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], center, null, Color.DarkRed, 0f, Vector2.Zero, 10, SpriteEffects.None, 1);
    }


    private void MakeEmitters()
    {
        DustWalkEmitter();

        Health.AmountDamageTaken += OnDamageTakenText;
    }

    private void OnDamageTakenText(int damage)
    {
        // Remove the active from the emitter, so that it dosent think it still owns them.
        damageTakenEmitter.SetParticleText(new TextOnSprite() { Text = damage.ToString() });
        damageTakenEmitter.EmitParticles();

        /* Take damage
         * Sets text
         * Starts Emitter
         * Emitter releases 1 particle
         * Waits for particle to end
         * But finnishes and turns off before particle disappears
         * 
         * Need to be able to see how many it has been hit, then release only one particle for each of those hits
         * Could be like the training dummy that saves the previous hits
         * Cant just turn down release per second since that makes the emitter count up to 1 only after it has been started
         * 
         * Could make the emitter forget the previous emitted so it only shows one again
         *      but this makes it so we have to change a bit in the update of the particle emitter
         * 
         * Cant make the max amount bigger
         * 
         * Can change the amount of time showed + emitter alive timer the same, to make the emitter work.
         *      But this makes it so we cant have more than one hit text on the character.
         *      And we have to show it for a very short time.
         *      
         * Need to make a method that ingnores the particles per second (maybe just 0 when createParticleEmitter)
         * A method that need to emit a single particle from the pool.
        */
    }

    public ParticleEmitter damageTakenEmitter;
    private void DustWalkEmitter()
    {
        GameObject go = EmitterFactory.CreateParticleEmitter("Text Damage Taken", new Vector2(200, -200), new Interval(180, 200), new Interval(-MathHelper.Pi, 0), 0, new Interval(500, 700), 100, -1, new Interval(-1f, 1f), new Interval(-0.001f, 0.001f));

        damageTakenEmitter = go.GetComponent<ParticleEmitter>();
        damageTakenEmitter.FollowGameObject(GameObject, new Vector2(0, 25));
        damageTakenEmitter.LayerName = LayerDepth.Text;
        damageTakenEmitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));
        damageTakenEmitter.AddModifier(new ColorRangeModifier(new Color[] { Color.DarkGray, Color.Moccasin, Color.Transparent }, new Color[] { Color.DarkRed, Color.Red, Color.OrangeRed, Color.Transparent }));

        damageTakenEmitter.AddBirthModifier(new ScaleBirthModifier(new Interval(4, 4)));
        damageTakenEmitter.AddModifier(new GravityModifier());

        GameWorld.Instance.Instantiate(go);
    }
}