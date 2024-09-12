using ShamansDungeon.CommandPattern;
using ShamansDungeon.ComponentPattern.Enemies;
using ShamansDungeon.ComponentPattern.PlayerClasses;
using ShamansDungeon.GameManagement;
using ShamansDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ShamansDungeon.ComponentPattern.Weapons;

/// <summary>
/// A class used to generate a collider that moves a rotation around its startPos
/// </summary>
public class CollisionRectangle
{
    public Rectangle Rectangle;
    public Vector2 StartRelativePos;
}

// Only happen on attack. Also add hands. Remove it from the player and use 2 hands.
// The hands should be given and made before making the weapon, as a part of which hands we should use.
// Use the clenched hand for the one for the weapon and relaxed hand for the other.
// A really nice to have to so make a trail behind the weapon when it swings:D Would be fun to make
public abstract class Weapon : Component
{
    #region Properties
    public Dictionary<WeaponAnimTypes, WeaponAnimation> Animations;
    public WeaponAnimTypes CurrentAnim;
    protected int CurrentAnimRepeats;
    public Player PlayerUser { get; set; }
    public Enemy EnemyUser { get; set; }
    protected Character User { get; private set; } // So avoid making the check if its a player or enemy
    public SpriteRenderer SpriteRenderer { get; set; }
    public float StartAnimationAngle { get; set; }
    protected double AttackedTotalElapsedTime { get; set; }
    private const float _baseEnemyWeakness = 0.3f; // What to time with, to make enemie attacks weaker.
    private float _enemyWeakness;
    public float EnemyWeakness {
        get
        {
            if (_enemyWeakness == 0) return _baseEnemyWeakness;
            return _enemyWeakness;
        }
        set
        {
            _enemyWeakness = value;
        }
    }
    public bool Attacking { get; protected set; }

    // A lot of this data is being copied on many different weapons, even though it has the same data.
    // Could use a GlobalPool or something that contains data, that are the same and wont be changed on each object
    protected SoundNames[] AttackSoundNames { get; set;}
    protected SoundNames[] AttackHitSoundNames { get; set; } = new SoundNames[]
    {
        SoundNames.WoodHitFlesh,
        SoundNames.WoodHitLeather,
        //SoundNames.WoodHitMetal,
    };

    public Vector2 LastOffSetPos;
    public Vector2 StartPosOffset = new(40, 20);
    public Vector2 StartRelativePos = new (0, 60), StartRelativeOffsetPos = new Vector2(0, -20);

    public float WeaponAngleToUser { get; set; }
    public bool LeftSide { get; private set; }
    protected double TimeBeforeNewDirection { get; set;}
    protected float animRotation, nextAnimRotation;
    public WeaponAnimTypes NextAnim { get; private set; }
    protected bool FinnishedAttack;
    /// <summary>
    /// The angle we can use for our animation, so we can lerp proberly.
    /// </summary>
    private float untouchedAngle;
    private int divideBy = 4;
    protected float FinalLerp { get; set; }

    protected double AttackTimer { get; set; }
    protected double AttackCooldown = 2.0;
    public bool UseAttackCooldown = true;

    private SoundEffectInstance _currentHitSound, _currentAttackSound;
    private float _hitMaxSound = 0.4f;
    private float _attackMaxSound = 0.8f;
    #endregion

    protected Weapon(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Awake()
    {
        AttackTimer = AttackCooldown;

        SpriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer.IsCentered = false;

        if (EnemyUser != null)
        {
            User = EnemyUser;
            EnemyWeaponSprite();
        }
        else
        {
            User = PlayerUser;
            PlayerWeaponSprite();
        }
        MakeHoldHand();
    }

    public override void Start()
    {

        if (Animations == null || Animations.Count == 0) return;

        animRotation = Animations[CurrentAnim].AmountOfRotation;

        NextAnim = CurrentAnim;

        nextAnimRotation = Animations[NextAnim].AmountOfRotation;
    }

    private void MakeHoldHand()
    {
        GameObject go = new();
        go.Transform.Position = GameObject.Transform.Position;
        go.Transform.Scale = User.GameObject.Transform.Scale;

        _holdHandSr = go.AddComponent<SpriteRenderer>();
        _holdHandSr.SetLayerDepth(SpriteRenderer.LayerDepth, 0.001f);
        _holdHandSr.SetSprite(TextureNames.HumanHandRight);
        _holdHand = go;
        GameWorld.Instance.Instantiate(go);
    }
    private GameObject _holdHand;
    private SpriteRenderer _holdHandSr;

    public override void Update()
    {
        // Check layer
        CheckLayerDepth();

        // Update normal timer
        if (UseAttackCooldown && AttackTimer < AttackCooldown)
            AttackTimer += GameWorld.DeltaTime;

        UpdateSound();
    }

    private void CheckLayerDepth()
    {
        // Offset for layerdepth, so the enemies are not figting for which is shown.
        float offSet = GameObject.Transform.Position.Y / 10_000_000f; // IMPORTANT, THIS CAN CHANGE WHAT LAYER ITS DRAWN ON

        if (GameObject.Transform.Position.Y < User.GameObject.Transform.Position.Y)
            offSet = -offSet;

        SpriteRenderer.SetLayerDepth(User.SpriteRenderer.LayerDepth, offSet);
        _holdHandSr.SetLayerDepth(SpriteRenderer.LayerDepth, 0.0001f);
    }

    public void StartAttack()
    {
        if (Attacking) return;
        if (Animations == null || Animations.Count == 0) return;

        // If the weapon uses cooldown between attacks, and the 
        if (UseAttackCooldown && AttackTimer < AttackCooldown) return;

        AttackTimer = 0;

        MoveWeaponAndAngle();
        
        Attacking = true;
        FinnishedAttack = false;

        ChangeWeaponAttacks();

        PlayAttackSound();

        //if (Animations == null || Animations.Count == 0) return;

        TimeBeforeNewDirection = Animations[CurrentAnim].TotalTime / 2;
        SetAttackDirection();
    }

    private void ChangeWeaponAttacks()
    {
        if (Animations == null || Animations.Count == 0) return;

        if (CurrentAnimRepeats == Animations[CurrentAnim].Repeats) // Change animation
        {
            CurrentAnimRepeats = 0; // Reset variable
            CurrentAnim = Animations[CurrentAnim].NextAnimation;
        }

        animRotation = Animations[CurrentAnim].AmountOfRotation;

        CurrentAnimRepeats++;

        // The animations to make sure that animation can lerp to the next starting angle.
        if (CurrentAnimRepeats == Animations[CurrentAnim].Repeats) 
        {
            NextAnim = Animations[CurrentAnim].NextAnimation;
        }
        else // The Animation is the same 
        {
            NextAnim = CurrentAnim;
        }

        nextAnimRotation = Animations[NextAnim].AmountOfRotation;

        if (UseAttackCooldown)
        {
            AttackCooldown = Animations[NextAnim].TotalTime;
        }
    }

    protected virtual void PlayerWeaponSprite()
    { }

    protected virtual void EnemyWeaponSprite()
    { }

    public virtual void SetAttackDirection() { }

    private void UpdateSound()
    {
        // Update normal attack sound
        GlobalSounds.ChangeSoundVolumeDistance(GameObject.Transform.Position, 50, 250, _hitMaxSound, _currentAttackSound);
        GlobalSounds.ChangeSoundVolumeDistance(GameObject.Transform.Position, 50, 250, _attackMaxSound, _currentHitSound);
    }

    protected void PlayAttackSound()
    {
        if (AttackSoundNames == null || AttackSoundNames.Length == 0) return;

        _currentAttackSound = GlobalSounds.PlayRandomizedSound(AttackSoundNames, 5, _attackMaxSound, true);
    }

    protected void PlayHitSound()
    {
        if (AttackHitSoundNames == null ||AttackHitSoundNames.Length == 0) return;

        _currentHitSound = GlobalSounds.PlayRandomizedSound(AttackHitSoundNames, 3, _hitMaxSound, true);
    }


    public void MoveWeaponAndAngle()
    {
        User.MoveWeaponPosAndAngle();
    }

    public void SetAngleToCorrectSide()
    {
        if (WeaponAngleToUser > 0.5 * MathHelper.Pi && WeaponAngleToUser < 1.5 * MathHelper.Pi)
        {
            WeaponAngleToUser += MathHelper.Pi;
            untouchedAngle = WeaponAngleToUser;

            LeftSide = true;
            SetAngleToFitWithNextAnimation();
            SetSpriteEffects(SpriteEffects.FlipHorizontally);
        }
        else
        {
            untouchedAngle = WeaponAngleToUser;

            LeftSide = false;
            SetAngleToFitWithNextAnimation();
            SetSpriteEffects(SpriteEffects.None);
        }
    }

    public void SetSpriteEffects(SpriteEffects newSpriteEffect)
    {
        SpriteRenderer.SpriteEffects = newSpriteEffect;
        _holdHandSr.SpriteEffects = newSpriteEffect;
    }
    public void SetColor(Color color)
    {
        SpriteRenderer.Color = color;
        _holdHandSr.Color = color;
    }
    public void SetPosition(Vector2 newPos)
    {
        GameObject.Transform.Position = newPos;
        _holdHand.Transform.Position = newPos;
    }
    public void SetRotation(float newRot)
    {
        GameObject.Transform.Rotation = newRot;
        _holdHand.Transform.Rotation = newRot;
    }
    public void ShowHideWeapon(bool show)
    {
        SpriteRenderer.ShouldDrawSprite = show;
        _holdHandSr.ShouldDrawSprite = show;
    }

    public void RemoveGameObject()
    {
        GameWorld.Instance.Destroy(GameObject);
        GameWorld.Instance.Destroy(_holdHand);
    }

    protected void SetStartAngleToNextAnim()
    {
        // If the rotation is the same dont do anything
        if (nextAnimRotation == animRotation) return;

        float leftOver = nextAnimRotation - MathHelper.Pi;

        // Resets angle
        WeaponAngleToUser = untouchedAngle;

        FinalLerp = LeftSide ? -nextAnimRotation : nextAnimRotation;
        AddLeftOverToAngle(LeftSide, leftOver, nextAnimRotation);

        StartAnimationAngle = WeaponAngleToUser;
    }

    private void SetAngleToFitWithNextAnimation()
    {
        float curRot;

        if (FinnishedAttack)
            curRot = nextAnimRotation;
        else
            curRot = animRotation;

        float leftOver = curRot - MathHelper.Pi;

        AddLeftOverToAngle(LeftSide, leftOver, curRot);
    }

    private void AddLeftOverToAngle(bool leftSide, float leftOver, float rotation)
    {
        WeaponAngleToUser += leftSide 
            ? (leftOver < 0 ? -rotation / divideBy :  leftOver / 2) 
            : (leftOver < 0 ?  rotation / divideBy : -leftOver / 2);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!InputHandler.Instance.DebugMode) return;
        Vector2 center = GameObject.Transform.Position - new Vector2(5, 5);
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], center, null, Color.Red, GameObject.Transform.Rotation, Vector2.Zero, 10, SpriteEffects.None, 1);
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], center, null, Color.Pink, GameObject.Transform.Rotation, Vector2.Zero, 10, SpriteEffects.None, 1);
    }
}