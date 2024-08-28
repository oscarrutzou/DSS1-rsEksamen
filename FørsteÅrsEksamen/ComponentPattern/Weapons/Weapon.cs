﻿using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern.Enemies;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DoctorsDungeon.ComponentPattern.Weapons;

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
    protected float StartAnimationAngle { get; set; }
    protected double AttackedTotalElapsedTime { get; set; }
    public static float EnemyWeakness = 2.5f; // What to divide with, to make enemie attacks weaker.
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

    protected Vector2 StartPosOffset = new(40, 20);
    private Vector2 lastOffSetPos, startRelativePos = new(0, 60), startRelativeOffsetPos = new Vector2(0, -20);
    private float _weaponAngleToUser { get; set; }
    protected bool LeftSide;
    protected double TimeBeforeNewDirection { get; set;}
    protected float animRotation, nextAnimRotation;
    public WeaponAnimTypes NextAnim { get; private set; }
    protected bool FinnishedAttack;
    private float untouchedAngle;
    private int divideBy = 4;
    protected float FinalLerp { get; set; }

    protected double AttackTimer { get; set; }
    protected double AttackCooldown = 2.0;
    public bool UseAttackCooldown = true;

    private SoundEffectInstance _currentHitSound, _currentAttackSound;
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
    }
    
    public override void Start()
    {
        animRotation = Animations[CurrentAnim].AmountOfRotation;

        NextAnim = CurrentAnim;

        nextAnimRotation = Animations[NextAnim].AmountOfRotation;
    }

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
    }

    public void StartAttack()
    {
        if (Attacking) return;

        // If the weapon uses cooldown between attacks, and the 
        if (UseAttackCooldown && AttackTimer < AttackCooldown) return;

        AttackTimer = 0;

        MoveWeaponAndAngle();
        
        Attacking = true;
        FinnishedAttack = false;

        ChangeWeaponAttacks();

        TimeBeforeNewDirection = Animations[CurrentAnim].TotalTime / 2;

        PlayAttackSound();

        SetAttackDirection();
    }

    private void ChangeWeaponAttacks()
    {
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

    protected virtual void SetAttackDirection()
    { }

    private void UpdateSound()
    {
        // Update normal attack sound
        GlobalSounds.ChangeSoundVolumeDistance(GameObject.Transform.Position, 50, 250, _hitMaxSound, _currentAttackSound);
        GlobalSounds.ChangeSoundVolumeDistance(GameObject.Transform.Position, 50, 250, _attackMaxSound, _currentHitSound);
    }

    private float _hitMaxSound = 0.4f;
    private float _attackMaxSound = 0.8f;
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
        Vector2 userPos;
        if (EnemyUser != null)
            userPos = EnemyUser.GameObject.Transform.Position;
        else
            userPos = PlayerUser.GameObject.Transform.Position;

        if (Attacking)
        {
            // Lock the offset
            GameObject.Transform.Position = userPos + lastOffSetPos;
            return;
        }

        if (EnemyUser != null && EnemyUser.CanAttack)
            _weaponAngleToUser = GetAngleToMouseEnemy(userPos);
        else if (PlayerUser != null)
            _weaponAngleToUser = GetAngleToMousePlayer();
        else // If the weapon shouldnt rotate to the player
            _weaponAngleToUser = 0f;
        
        // Adjust the angle to be in the range of 0 to 2π
        if (_weaponAngleToUser < 0)
        {
            _weaponAngleToUser += 2 * MathHelper.Pi;
        }

        lastOffSetPos = BaseMath.Rotate(startRelativePos, _weaponAngleToUser - MathHelper.PiOver2) + startRelativeOffsetPos;
        GameObject.Transform.Position = userPos + lastOffSetPos;

        SetAngleToCorrectSide();
        if (EnemyUser != null && !EnemyUser.CanAttack) return;

        StartAnimationAngle = _weaponAngleToUser;
        GameObject.Transform.Rotation = StartAnimationAngle;
    }

    private void SetAngleToCorrectSide()
    {
        if (_weaponAngleToUser > 0.5 * MathHelper.Pi && _weaponAngleToUser < 1.5 * MathHelper.Pi)
        {
            _weaponAngleToUser += MathHelper.Pi;
            untouchedAngle = _weaponAngleToUser;

            LeftSide = true;
            SetAngleToFitWithNextAnimation();
            SpriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
        }
        else
        {
            untouchedAngle = _weaponAngleToUser;

            LeftSide = false;
            SetAngleToFitWithNextAnimation();
            SpriteRenderer.SpriteEffects = SpriteEffects.None;
        }
    }

    protected void SetStartAngleToNextAnim()
    {
        // If the rotation is the same dont do anything
        if (nextAnimRotation == animRotation) return;

        float leftOver = nextAnimRotation - MathHelper.Pi;

        // Resets angle
        _weaponAngleToUser = untouchedAngle;

        FinalLerp = LeftSide ? -nextAnimRotation : nextAnimRotation;
        AddLeftOverToAngle(LeftSide, leftOver, nextAnimRotation);

        StartAnimationAngle = _weaponAngleToUser;
    }

    private void SetAngleToFitWithNextAnimation()
    {
        float curRot;

        if (FinnishedAttack)
        {
            curRot = nextAnimRotation;
        }
        else
        {
            curRot = animRotation;
        }

        float leftOver = curRot - MathHelper.Pi;

        AddLeftOverToAngle(LeftSide, leftOver, curRot);
    }

    private void AddLeftOverToAngle(bool leftSide, float leftOver, float rotation)
    {
        _weaponAngleToUser += leftSide ? (leftOver < 0 ? -rotation / divideBy : leftOver / 2) :
                            (leftOver < 0 ? rotation / divideBy : -leftOver / 2);
    }

    private float GetAngleToMousePlayer()
    {
        Vector2 mouseInUI = InputHandler.Instance.MouseOnUI;
        return (float)Math.Atan2(mouseInUI.Y, mouseInUI.X);
    }

    private float GetAngleToMouseEnemy(Vector2 userPos)
    {
        Player target = EnemyUser.Player;

        Vector2 relativePos = target.GameObject.Transform.Position - userPos;
        return (float)Math.Atan2(relativePos.Y, relativePos.X);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!InputHandler.Instance.DebugMode) return;
        Vector2 center = GameObject.Transform.Position - new Vector2(5, 5);
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], center, null, Color.Red, GameObject.Transform.Rotation, Vector2.Zero, 10, SpriteEffects.None, 1);
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], center, null, Color.Pink, GameObject.Transform.Rotation, Vector2.Zero, 10, SpriteEffects.None, 1);
    }
}