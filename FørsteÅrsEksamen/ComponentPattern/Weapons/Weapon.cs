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

public enum WeaponAnimTypes
{
    Light,
    Medium,
    Heavy,
    // Maybe special or something
}

public delegate float AnimationFunction(float x);

// Play with some other methods, for different weapons, to make them feel slow or fast https://easings.net/
public class WeaponAnimation
{
    public float TotalTime;
    public float AmountOfRotation;
    public int Repeats;
    public int Damage;
    public AnimationFunction AnimationMethod; // Delegate field
    public WeaponAnimTypes NextAnimation;

    public WeaponAnimation(float totalTime,
                           float amountOfRotation,
                           int damage,
                           AnimationFunction animationMethod,
                           WeaponAnimTypes nextAnimation,
                           int repeats = 1)
    {
        TotalTime = totalTime;
        AmountOfRotation = amountOfRotation;
        Damage = damage;
        AnimationMethod = animationMethod; // Assign the delegate
        NextAnimation = nextAnimation;
        Repeats = repeats;
    }

    public float CalculateAnimation(float x)
    {
        return AnimationMethod(x);
    }
}

// NexAnim
// A weapon will always have a light attack. It goes though the enum, enum.default = first in.
// Need to save how many that attack has been picked, with how many times it should repeat
// Could do it with a int that

// Erik
// Notes for what to add or change to the Weapon.
// Only happen on attack. Also add hands. Remove it from the player and use 2 hands.
// The hands should be given and made before making the weapon, as a part of which hands we should use.
// Use the clenched hand for the one for the weapon and relaxed hand for the other.
// A really nice to have to so make a trail behind the weapon when it swings:D Would be fun to make
public abstract class Weapon : Component
{
    public Dictionary<WeaponAnimTypes, WeaponAnimation> Animations;
    public WeaponAnimTypes CurrentAnim;
    protected int CurrentAnimRepeats;
    public Player PlayerUser { get; set; }
    public Enemy EnemyUser { get; set; }

    protected SpriteRenderer spriteRenderer;

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


    protected Weapon(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Awake()
    {
        spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.SetLayerDepth(LayerDepth.PlayerWeapon); // Should not matter?
        spriteRenderer.IsCentered = false;

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

        Attacking = true;
        FinnishedAttack = false;

        ChangeWeaponAttacks();
        MoveWeapon(true);

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

    public void MoveWeapon(bool moveOnlyAngle = false)
    {
        Vector2 userPos;
        if (EnemyUser != null)
            userPos = EnemyUser.GameObject.Transform.Position;
        else
            userPos = PlayerUser.GameObject.Transform.Position;

        if (Attacking && !moveOnlyAngle)
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

        if (!moveOnlyAngle)
        {
            lastOffSetPos = BaseMath.Rotate(startRelativePos, angle - MathHelper.PiOver2) + startRelativeOffsetPos;
            GameObject.Transform.Position = userPos + lastOffSetPos;
        }

        // Set the StartAnimationAngle based on the adjusted angle
        if (angle > 0.5 * MathHelper.Pi && angle < 1.5 * MathHelper.Pi)
        {
            angle += MathHelper.Pi;
            untouchedAngle = angle;

            SetAngleToFitWithNextAnimation(true);

            LeftSide = true;
            spriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
        }
        else
        {
            untouchedAngle = angle;

            SetAngleToFitWithNextAnimation(false);

            LeftSide = false;
            spriteRenderer.SpriteEffects = SpriteEffects.None;
        }

        StartAnimationAngle = angle;

        GameObject.Transform.Rotation = StartAnimationAngle;
    }

    private float untouchedAngle;
    protected void SetStartAngleToNextAnim()
    {
        // If the rotation is the same dont do anything
        if (nextAnimRotation == animRotation) return;

        float leftOver = nextAnimRotation - MathHelper.Pi;

        // Resets angle
        angle = untouchedAngle;

        if (LeftSide)
        {
            FinalLerp = -nextAnimRotation;

            if (leftOver < 0)
                angle -= nextAnimRotation / divideBy;
            else
                angle += leftOver / 2;
        }
        else
        {
            FinalLerp = nextAnimRotation;

            if (leftOver < 0)
                angle += nextAnimRotation / divideBy;
            else
                angle -= leftOver / 2;
        }

        StartAnimationAngle = angle;
    }

    int divideBy = 4;
    protected float FinalLerp { get; set; }

    private void SetAngleToFitWithNextAnimation(bool isLeft)
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

        if (isLeft)
        {
            if (leftOver < 0)
                angle -= curRot / divideBy;
            else
                angle += leftOver / 2;
        }
        else
        {
            if (leftOver < 0)
                angle += curRot / divideBy;
            else
                angle -= leftOver / 2;
        }
    }
    /* 
     * Light = 1 start, slut 3
     * Med = 0 start
     * 
     * Lock når angle 0 og den lige har attacked
      * Efter når den er færdig atttacking:
      *      Check new angle, og lerp fra gammel til ny angle
      *      Hold attack locked imens
      *      Når den rammer omkring ny angle, åben attack.
     */

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