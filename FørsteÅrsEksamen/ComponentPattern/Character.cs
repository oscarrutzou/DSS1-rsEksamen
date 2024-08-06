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
using SharpDX.X3DAudio;

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
        Health.OnZeroHealth += OnDie;
        //Health.OnDamageTaken += OnDamageTaken;
        //Health.OnResetColor += OnResetColor;
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

        //Health.OnDamageTaken -= OnDamageTaken;
        //Health.OnResetColor -= OnResetColor;
        Health.OnZeroHealth -= OnDie;
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
        DamageTakenEmitter.LayerName = LayerDepth.PlayerWeapon;
        DamageTakenEmitter.SetParticleText(new TextOnSprite() { Text = damage.ToString() });
        DamageTakenEmitter.EmitParticles();
    }

    public ParticleEmitter DamageTakenEmitter { get; private set; }
    protected Color[] DamageTakenAmountTextColor = new Color[] { Color.OrangeRed, Color.DarkRed, Color.Transparent };

    private void DustWalkEmitter()
    {
        GameObject go = EmitterFactory.TextDamageEmitter(DamageTakenAmountTextColor, GameObject, new Vector2(-20, -95), new RectangleOrigin(50, 5));
        DamageTakenEmitter = go.GetComponent<ParticleEmitter>();

        GameWorld.Instance.Instantiate(go);
    }


}