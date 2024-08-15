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
using DoctorsDungeon.ComponentPattern.Path;

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
    public static Vector2 LargeSpriteOffSet = new(0, -32); // Move the animation up more since its a 64x64 insted of 32x32 canvans, for the Run and Death.

    public CharacterState State { get; protected set; } = CharacterState.Moving; // We use the method SetState, to we can change the animations and other variables.
    public Vector2 Direction { get; protected set; }
    protected AnimationDirectionState DirectionState = AnimationDirectionState.Right;


    protected int Speed { get; set; }
    public int CollisionNr { get; set; }
    protected Grid Grid; 


    #endregion Properties

    public Character(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Awake()
    {
        Grid = GridManager.Instance.CurrentGrid;

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

    private ParticleEmitter dustCloudEmitter;

    public Cell SetStartCollisionNr()
    {
        if (Grid == null) return null;

        GameObject currentCellGo = Grid.GetCellGameObjectFromPoint(GameObject.Transform.GridPosition);
        GameObject.Transform.Position = currentCellGo.Transform.Position;
        Cell cell = currentCellGo.GetComponent<Cell>();
        CollisionNr = cell.CollisionNr;
        return cell;
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

        if (State == CharacterState.Moving)
            dustCloudEmitter.StartEmitter();
        else
            dustCloudEmitter.StopEmitter();

        switch (State)
        {
            case CharacterState.Idle:
                Animator.PlayAnimation(CharacterStateAnimations[State]);
                SpriteRenderer.SetOriginOffset(new Vector2(16, 16));

                SpriteRenderer.DrawPosOffSet = SmallSpriteOffset;
                break;

            case CharacterState.Moving:
                Animator.PlayAnimation(CharacterStateAnimations[State]);

                SpriteRenderer.SetOriginOffset(new Vector2(32, 48));
                SpriteRenderer.DrawPosOffSet = LargeSpriteOffSet;
                break;

            case CharacterState.Attacking:
                Animator.PlayAnimation(CharacterStateAnimations[CharacterState.Idle]); // Just uses the Idle since we have no attacking animation
                SpriteRenderer.SetOriginOffset(new Vector2(16, 16));

                SpriteRenderer.DrawPosOffSet = SmallSpriteOffset;
                break;

            case CharacterState.Dead:
                Animator.PlayAnimation(CharacterStateAnimations[State]);

                SpriteRenderer.SetOriginOffset(new Vector2(32, 48));


                SpriteRenderer.DrawPosOffSet = LargeSpriteOffSet;
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

    public virtual void Attack()
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

        Health.OnZeroHealth -= OnDie;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!InputHandler.Instance.DebugMode) return;
        // Draws the center of the sprite, with a 10x10 centered pixel
        Vector2 center = GameObject.Transform.Position - new Vector2(5, 5);
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], center, null, Color.DarkRed, 0f, Vector2.Zero, 10, SpriteEffects.None, 1);
    }

    private void MakeEmitters()
    {
        MakeDustEmitter();
        MakeDamageTakenEmitter();

        Health.AmountDamageTaken += OnDamageTakenText;
    }

    private void MakeDustEmitter()
    {
        GameObject go = EmitterFactory.CreateParticleEmitter("Dust Cloud", new Vector2(200, -200), new Interval(100, 150), new Interval(-MathHelper.Pi, 0), 30, new Interval(500, 1000), 1000, -1, new Interval(-MathHelper.Pi, 0), new Interval(-0.01, 0.01));

        dustCloudEmitter = go.GetComponent<ParticleEmitter>();
        dustCloudEmitter.FollowGameObject(GameObject, new Vector2(0, 25));
        dustCloudEmitter.LayerName = LayerDepth.EnemyUnder;
        dustCloudEmitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel4x4));
        dustCloudEmitter.AddModifier(new ColorRangeModifier(new Color[] { new(142, 94, 52), new(82, 61, 42), Color.Transparent }));

        dustCloudEmitter.AddBirthModifier(new ScaleBirthModifier(new Interval(1, 2)));
        dustCloudEmitter.AddModifier(new GravityModifier(20f));

        dustCloudEmitter.Origin = new RectangleOrigin(80, 30);

        GameWorld.Instance.Instantiate(go);
    }

    private void OnDamageTakenText(int damage)
    {
        DamageTakenEmitter.LayerName = LayerDepth.PlayerWeapon;
        DamageTakenEmitter.SetParticleText(new TextOnSprite() { Text = damage.ToString() });
        DamageTakenEmitter.EmitParticles();
    }

    public ParticleEmitter DamageTakenEmitter { get; private set; }
    protected Color[] DamageTakenAmountTextColor = new Color[] { Color.OrangeRed, Color.DarkRed, Color.Transparent };

    private void MakeDamageTakenEmitter()
    {
        GameObject go = EmitterFactory.TextDamageEmitter(DamageTakenAmountTextColor, GameObject, new Vector2(-20, -95), new RectangleOrigin(50, 5));
        DamageTakenEmitter = go.GetComponent<ParticleEmitter>();

        GameWorld.Instance.Instantiate(go);
    }


}