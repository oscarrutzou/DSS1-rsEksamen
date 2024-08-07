using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.GameManagement.Scenes;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern.Weapons.MeleeWeapons;

// Erik
public abstract class MeleeWeapon : Weapon
{
    protected bool IsRotatingBack;
    protected List<CollisionRectangle> WeaponColliders = new();

    private List<GameObject> hitGameObjects { get; set; } = new(); 

    private float rotateBackStartRotation;
    private double resetHitObjectsTimer, timeBeforeCanHitAfterRotatingBack = 0.3;
    private bool firstResetHittedObjects, secondResetHittedObjects;
    protected MeleeWeapon(GameObject gameObject) : base(gameObject)
    {
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

    public override void Update()
    {
        if (Attacking)
        {
            TotalElapsedTime += GameWorld.DeltaTime;
            AttackAnimation();

            CheckCollisionAndDmg();
        }

        UpdateCollisionBoxesPos(GameObject.Transform.Rotation);
    }


    private void AttackAnimation()
    {
        // This should be changed to another animation method if its a stab attack
        if (Animations[CurrentAnim].SelectedAttackType == WeaponAnimAttackTypes.Slash)
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
        // First rotate current angle to start angle of the anim, before attacking
        if (!IsRotatingBack && TotalElapsedTime >= TimeBeforeNewDirection)
        {
            PlayAttackSound();

            TotalElapsedTime = 0f; // Reset totalElapsedTime
            IsRotatingBack = true;
            //hitGameObjects = new(); // Reset hit gameobjects so we can hit when it goes back again
            secondResetHittedObjects = false;

            SetStartAngleToNextAnim(); // Changes the StartAnimationAngle so it rotates to the next animation start, insted of snapping to the place after
            // Need to also set the new start point
            rotateBackStartRotation = GameObject.Transform.Rotation;

            // Makes the weapon flip when rotating back
            if (SpriteRenderer.SpriteEffects == SpriteEffects.FlipHorizontally)
                SpriteRenderer.SpriteEffects = SpriteEffects.None;
            else if (SpriteRenderer.SpriteEffects == SpriteEffects.None)
                SpriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
        }

        ResetHittedGameObjects();

        float normalizedTime = (float)TotalElapsedTime / (float)TimeBeforeNewDirection;
        float easedTime; // maybe switch between them.
        float finalLerp = StartAnimationAngle;

        if (!IsRotatingBack)
        {
            finalLerp += FinalLerp; // The first rotation
            // Down attack
            easedTime = Animations[CurrentAnim].AnimationMethod(normalizedTime);
            GameObject.Transform.Rotation = MathHelper.Lerp(StartAnimationAngle, finalLerp, easedTime);
        }
        else
        {
            // Second rotation to rotate to the start of the next rotation
            //Up attack
            easedTime = Animations[CurrentAnim].AnimationMethod(normalizedTime);
            GameObject.Transform.Rotation = MathHelper.Lerp(rotateBackStartRotation, StartAnimationAngle, easedTime);
        }

        if (Math.Abs(GameObject.Transform.Rotation - StartAnimationAngle) < 0.1f && IsRotatingBack)
        {
            IsRotatingBack = false;
            Attacking = false;
            FinnishedAttack = true;
        }
    }


    private void AttackAnimStab()
    {
        /*
         * How should the stab work.
         */
    }

    public void CheckCollisionAndDmg()
    {
        GameObjectTypes type;
        if (EnemyUser != null)
            type = GameObjectTypes.Player;
        else
            type = GameObjectTypes.Enemy;

        foreach (GameObject otherGo in SceneData.GameObjectLists[type])
        {
            if (!otherGo.IsEnabled || hitGameObjects.Contains(otherGo)) continue;

            Collider otherCollider = otherGo.GetComponent<Collider>();

            if (otherCollider == null) continue;
            foreach (CollisionRectangle weaponRectangle in WeaponColliders)
            {
                if (hitGameObjects.Contains(otherGo)) break; // Need to check again here so it dosent attack twice

                if (weaponRectangle.Rectangle.Intersects(otherCollider.CollisionBox))
                {
                    hitGameObjects.Add(otherGo);
                    DealDamage(otherGo);
                    break;
                }
            }
        }
    }

    private void ResetHittedGameObjects()
    {
        // Reset GameObjects when Rotate down
        if (!IsRotatingBack && !firstResetHittedObjects)
        {
            resetHitObjectsTimer += GameWorld.DeltaTime;
            if (resetHitObjectsTimer > timeBeforeCanHitAfterRotatingBack)
            {
                hitGameObjects = new();
                firstResetHittedObjects = true;
                resetHitObjectsTimer = 0;
            }
        }

        // Reset GameObjects when Rotate back
        if (IsRotatingBack && !secondResetHittedObjects)
        {
            resetHitObjectsTimer += GameWorld.DeltaTime;
            if (resetHitObjectsTimer > timeBeforeCanHitAfterRotatingBack)
            {
                hitGameObjects = new();
                secondResetHittedObjects = true;
                resetHitObjectsTimer = 0;
            }
        }
    }

    protected override void SetAttackDirection()
    {
        TotalElapsedTime = 0f;

        firstResetHittedObjects = false;
        resetHitObjectsTimer = 0;

        timeBeforeCanHitAfterRotatingBack = Math.Max(0.15f, TimeBeforeNewDirection / 3); 

        if (LeftSide)
            FinalLerp = -Animations[CurrentAnim].AmountOfRotation;
        else
            FinalLerp = Animations[CurrentAnim].AmountOfRotation;
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
        SpriteRenderer.OriginOffSet = origin;
        SpriteRenderer.DrawPosOffSet = -origin;
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