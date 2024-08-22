using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.GameManagement.Scenes;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons;

public class HitDamagePackage
{
    public double TimeOfHit {  get; set; }
    public bool WasRotatingBack { get; set; }
    public HitDamagePackage(double timeOfHit, bool wasRotatingBack)
    {
        TimeOfHit = timeOfHit;
        WasRotatingBack = wasRotatingBack;
    }
}

// Erik
public abstract class MeleeWeapon : Weapon
{
    protected bool IsRotatingBack; 
    protected List<CollisionRectangle> WeaponColliders = new();
    protected double MinimumTimeBetweenHits = 0.3f;

    private Dictionary<GameObject, HitDamagePackage> hitGameObjects { get; set; } = new(); 

    private float rotateBackStartRotation;
    private double timeCooldownBetweenHits { get; set; }
    private double totalElapsedTime;
    public float NormalizedFullAttack { get; private set; } // The normalized value for how far the attack is to be dont
    private float _fullAttackTimer;
    protected bool CanDealDamage = true;

    protected MeleeWeapon(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Update()
    {
        base.Update();

        totalElapsedTime += GameWorld.DeltaTime;
        ResetHittedGameObjects();

        if (Attacking)
        {
            AttackAnimation();

            CheckCollisionAndDmg();
        }

        UpdateCollisionBoxesPos(GameObject.Transform.Rotation);
    }


    private void AttackAnimation()
    {
        AttackedTotalElapsedTime += GameWorld.DeltaTime;

        // This should be changed to another animation method if its a stab attack
        if (Animations[CurrentAnim].SelectedAttackType == WeaponAnimAttackTypes.TwoWaySlash)
        {
            AttackAnimSlash();
        }
        else if (Animations[CurrentAnim].SelectedAttackType == WeaponAnimAttackTypes.Stab)
        {
            AttackAnimStab();
        }
    }


    private void AttackAnimSlash()
    {
        _fullAttackTimer += (float)GameWorld.DeltaTime;
        NormalizedFullAttack = _fullAttackTimer / Animations[CurrentAnim].TotalTime; 

        // First rotate current angle to start angle of the anim, before attacking
        if (!IsRotatingBack && AttackedTotalElapsedTime >= TimeBeforeNewDirection)
        {
            PlayAttackSound();

            AttackedTotalElapsedTime = 0f; // Reset totalElapsedTime
            IsRotatingBack = true;

            SetStartAngleToNextAnim(); // Changes the StartAnimationAngle so it rotates to the next animation start, insted of snapping to the place after
            // Need to also set the new start point
            rotateBackStartRotation = GameObject.Transform.Rotation;

            // Makes the weapon flip when rotating back
            if (SpriteRenderer.SpriteEffects == SpriteEffects.FlipHorizontally)
                SpriteRenderer.SpriteEffects = SpriteEffects.None;
            else if (SpriteRenderer.SpriteEffects == SpriteEffects.None)
                SpriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
        }

        float normalizedAttackingTime = (float)AttackedTotalElapsedTime / (float)TimeBeforeNewDirection;
        float easedTime = Animations[CurrentAnim].AnimationMethod(normalizedAttackingTime); // maybe switch between them.
        float adjustedEasedTime = easedTime * (normalizedAttackingTime);
        float finalLerp = StartAnimationAngle;

        if (!IsRotatingBack)
        {
            finalLerp += FinalLerp; // The first rotation
            // Down attack
            GameObject.Transform.Rotation = MathHelper.Lerp(StartAnimationAngle, finalLerp, adjustedEasedTime);
        }
        else
        {
            // Second rotation to rotate to the start of the next rotation
            //Up attack
            GameObject.Transform.Rotation = MathHelper.Lerp(rotateBackStartRotation, StartAnimationAngle, adjustedEasedTime);
        }

        // Reset
        if (NormalizedFullAttack >= 1f)
        {
            IsRotatingBack = false;
            Attacking = false;
            FinnishedAttack = true;
            ResetAttackTimers();
        }
    }

    private void ResetAttackTimers()
    {
        _fullAttackTimer = 0f;
        NormalizedFullAttack = 0f;
        AttackedTotalElapsedTime = 0f;
    }

    protected override void SetAttackDirection()
    {
        ResetAttackTimers();

        timeCooldownBetweenHits = Math.Max(MinimumTimeBetweenHits, TimeBeforeNewDirection / 3);

        if (LeftSide)
            FinalLerp = -Animations[CurrentAnim].AmountOfRotation;
        else
            FinalLerp = Animations[CurrentAnim].AmountOfRotation;
    }

    private void AttackAnimStab()
    {
        /*
         * How should the stab work.
         */
    }

    public void CheckCollisionAndDmg()
    {
        if (!CanDealDamage) return;

        GameObjectTypes type;
        if (EnemyUser != null)
            type = GameObjectTypes.Player;
        else
            type = GameObjectTypes.Enemy;

        foreach (GameObject otherGo in SceneData.GameObjectLists[type])
        {
            if (!otherGo.IsEnabled || hitGameObjects.ContainsKey(otherGo)) continue;
            
            Collider otherCollider = otherGo.GetComponent<Collider>();

            if (otherCollider == null) continue;
            foreach (CollisionRectangle weaponRectangle in WeaponColliders)
            {
                if (hitGameObjects.ContainsKey(otherGo)) break; // Need to check again here so it dosent attack twice

                if (weaponRectangle.Rectangle.Intersects(otherCollider.CollisionBox))
                {
                    hitGameObjects.Add(otherGo, new(totalElapsedTime, IsRotatingBack));
                    DealDamage(otherGo);
                    break;
                }
            }
        }
    }

    private void ResetHittedGameObjects()
    {
        Dictionary<GameObject, HitDamagePackage> temp = hitGameObjects;
        foreach (GameObject go in temp.Keys)
        {
            HitDamagePackage package = temp[go];
            double difference = totalElapsedTime - package.TimeOfHit;
            // Need also to check that its not the same IsRotatingBack
            if (difference >= timeCooldownBetweenHits && IsRotatingBack != package.WasRotatingBack)
            {
                hitGameObjects.Remove(go);
            }
        }

    }

    public void DealDamage(GameObject damageGo)
    {
        Health health = damageGo.GetComponent<Health>();

        // Float so we can divide with enemy weakness
        float damage = Animations[CurrentAnim].Damage;
        if (EnemyUser != null)
            damage /= EnemyWeakness;

        health.TakeDamage((int)damage);
    }


    #region Weapon Colliders

    private void UpdateCollisionBoxesPos(float rotation)
    {
        foreach (CollisionRectangle collisionRectangle in WeaponColliders)
        {
            // Calculate the position relative to the center of the weapon
            Vector2 relativePos = collisionRectangle.StartRelativePos;

            // Rotate the relative position
            Vector2 newPos = BaseMath.Rotate(relativePos, rotation);

            // Set the collision rectangle position based on the rotated relative position
            collisionRectangle.Rectangle.X = (int)(GameObject.Transform.Position.X + newPos.X) - collisionRectangle.Rectangle.Width / 2;
            collisionRectangle.Rectangle.Y = (int)(GameObject.Transform.Position.Y + newPos.Y) - collisionRectangle.Rectangle.Height / 2;
        }
    }

    /// <summary>
    /// To make colliders for the weapon.
    /// </summary>
    /// <param name="origin">How far the origin is from the top left corner. Should have a -0.5f in X to make it centered.</param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="heightFromOriginToHandle">Height from the origin on the sprite to the end of the handle</param>
    /// <param name="amountOfColliders"></param>
    protected void SetStartColliders(Vector2 origin, int width, int height, int heightFromOriginToHandle, int amountOfColliders)
    {
        SpriteRenderer.SetOriginOffset(origin);
        AddWeaponColliders(width, height, heightFromOriginToHandle, amountOfColliders);
    }

    /// <summary>
    /// The colliders on our weapon, used for collision between characters
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="heightFromOriginToHandle">Height from the origin on the sprite to the end of the handle</param>
    /// <param name="amountOfColliders"></param>
    private void AddWeaponColliders(int width, int height, int heightFromOriginToHandle, int amountOfColliders)
    {
        Vector2 pos = GameObject.Transform.Position;
        Vector2 scale = GameObject.Transform.Scale;

        pos += new Vector2(0, -heightFromOriginToHandle * scale.Y); // Adds the height from origin to handle

        // Adds the weapon colliders
        for (int i = 0; i < amountOfColliders; i++)
        {
            pos += new Vector2(0, -height * scale.Y);

            WeaponColliders.Add(new CollisionRectangle()
            {
                Rectangle = MakeRec(pos, width, height, scale),
                StartRelativePos = pos
            });
        }
    }

    private Rectangle MakeRec(Vector2 pos, int width, int height, Vector2 scale) => new Rectangle((int)pos.X, (int)pos.Y, width * (int)scale.X, (int)scale.Y * height);

    #endregion Weapon Colliders

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!InputHandler.Instance.DebugMode) return;

        foreach (CollisionRectangle collisionRectangle in WeaponColliders)
        {
            Collider.DrawRectangleNoSprite(collisionRectangle.Rectangle, Color.OrangeRed, spriteBatch);
        }
        base.Draw(spriteBatch);
    }
}