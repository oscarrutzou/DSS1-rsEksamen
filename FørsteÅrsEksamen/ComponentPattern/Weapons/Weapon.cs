using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern.Enemies;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
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

    public SpriteRenderer SpriteRenderer { get; set; }

    protected float StartAnimationAngle { get; set; }

    protected float TotalElapsedTime;
    protected float EnemyWeakness = 2.5f; // What to divide with, to make enemie attacks weaker.

    protected bool Attacking;

    // A lot of this data is being copied on many different weapons, even though it has the same data.
    // Could use a GlobalPool or something that contains data, that are the same and wont be changed on each object
    protected SoundNames[] AttackSoundNames;

    protected Vector2 StartPosOffset = new(40, 20);
    private Vector2 lastOffSetPos, startRelativePos = new(0, 60), startRelativeOffsetPos = new Vector2(0, -20);
    public float angle; // Public for test
    protected bool LeftSide;
    protected float TimeBeforeNewDirection;
    protected float animRotation, nextAnimRotation;
    public WeaponAnimTypes NextAnim { get; private set; }
    protected bool FinnishedAttack;
    private float untouchedAngle;
    int divideBy = 4;
    protected float FinalLerp { get; set; }
    #endregion

    protected Weapon(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Awake()
    {
        SpriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer.SetLayerDepth(LayerDepth.PlayerWeapon); // Should not matter?
        SpriteRenderer.IsCentered = false;

        if (EnemyUser != null)
        {
            EnemyWeaponSprite();
        }
        else
        {
            PlayerWeaponSprite();
        }
    }

    public void StartAttack()
    {
        if (Attacking) return;

        MoveWeaponAndAngle();
        
        Attacking = true;
        FinnishedAttack = false;

        ChangeWeaponAttacks();

        TimeBeforeNewDirection = Animations[CurrentAnim].TotalTime / 2;

        PlayAttackSound();

        SetAttackDirection();
    }

    public override void Start()
    {
        animRotation = Animations[CurrentAnim].AmountOfRotation;

        NextAnim = CurrentAnim;

        nextAnimRotation = Animations[NextAnim].AmountOfRotation;
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
    }


    protected virtual void PlayerWeaponSprite()
    { }

    protected virtual void EnemyWeaponSprite()
    { }

    protected virtual void SetAttackDirection()
    { }

    protected void PlayAttackSound()
    {
        if (AttackSoundNames == null || AttackSoundNames.Length == 0) return;

        GlobalSounds.PlayRandomizedSound(AttackSoundNames, 5, 1f, true);
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

        if (EnemyUser != null)
            angle = GetAngleToMouseEnemy(userPos);
        else
            angle = GetAngleToMousePlayer();

        // Adjust the angle to be in the range of 0 to 2π
        if (angle < 0)
        {
            angle += 2 * MathHelper.Pi;
        }

        lastOffSetPos = BaseMath.Rotate(startRelativePos, angle - MathHelper.PiOver2) + startRelativeOffsetPos;
        GameObject.Transform.Position = userPos + lastOffSetPos;


        SetAngleToCorrectSide();

        StartAnimationAngle = angle;

        GameObject.Transform.Rotation = StartAnimationAngle;
    }

    private void SetAngleToCorrectSide()
    {
        if (angle > 0.5 * MathHelper.Pi && angle < 1.5 * MathHelper.Pi)
        {
            angle += MathHelper.Pi;
            untouchedAngle = angle;

            LeftSide = true;
            SetAngleToFitWithNextAnimation();
            SpriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
        }
        else
        {
            untouchedAngle = angle;

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
        angle = untouchedAngle;

        FinalLerp = LeftSide ? -nextAnimRotation : nextAnimRotation;
        AddLeftOverToAngle(LeftSide, leftOver, nextAnimRotation);

        StartAnimationAngle = angle;
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
        angle += leftSide ? (leftOver < 0 ? -rotation / divideBy : leftOver / 2) :
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