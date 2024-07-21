using DoctorsDungeon.CommandPattern;
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
    protected float FinalLerp;
    protected bool IsRotatingBack;
    protected List<CollisionRectangle> WeaponColliders = new();

    private List<GameObject> hitGameObjects = new();


    protected MeleeWeapon(GameObject gameObject) : base(gameObject)
    {
    }

    public void DealDamage(GameObject damageGo)
    {
        Character damageGoHealth = damageGo.GetComponent<Character>();

        // Float so we can divide with enemy weakness
        float damage = Animations[CurrentAnim].Damage;
        if (EnemyUser != null)
            damage /= EnemyWeakness;

        damageGoHealth.TakeDamage((int)damage);
    }

    public override void Update(GameTime gameTime)
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
        // First rotate current angle to start angle of the anim, before attacking
        // Animations[CurrentAnim].AmountOfRotation;

        if (!IsRotatingBack && TotalElapsedTime >= TimeBeforeNewDirection)
        {
            PlayAttackSound();

            TotalElapsedTime = 0f; // Reset totalElapsedTime
            IsRotatingBack = true;
            hitGameObjects = new(); // Reset hit gameobjects so we can hit when it goes back again

            // Makes the weapon flip when rotating back
            if (spriteRenderer.SpriteEffects == SpriteEffects.FlipHorizontally)
                spriteRenderer.SpriteEffects = SpriteEffects.None;
            else if (spriteRenderer.SpriteEffects == SpriteEffects.None)
                spriteRenderer.SpriteEffects = SpriteEffects.FlipHorizontally;
        }

        float normalizedTime = TotalElapsedTime / TimeBeforeNewDirection;

        float easedTime; // maybe switch between them.
        float finalLerp = StartAnimationAngle + FinalLerp;

        if (!IsRotatingBack)
        {
            // Down attack
            easedTime = Animations[CurrentAnim].AnimationMethod(normalizedTime);
            GameObject.Transform.Rotation = MathHelper.Lerp(StartAnimationAngle, finalLerp, easedTime);
        }
        else
        {
            //Up attack
            easedTime = Animations[CurrentAnim].AnimationMethod(normalizedTime);
            GameObject.Transform.Rotation = MathHelper.Lerp(finalLerp, StartAnimationAngle, easedTime);
        }

        if (Math.Abs(GameObject.Transform.Rotation - StartAnimationAngle) < 0.1f && IsRotatingBack)
        {
            IsRotatingBack = false;
            Attacking = false;
            finnishedAttack = true;
        }
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

    protected override void SetAttackDirection()
    {
        TotalElapsedTime = 0f;

        hitGameObjects = new();
        if (LeftSide)
        {
            // Left
            FinalLerp = -Animations[CurrentAnim].AmountOfRotation;
        }
        else
        {
            // Right
            FinalLerp = Animations[CurrentAnim].AmountOfRotation;
        }
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
        spriteRenderer.OriginOffSet = origin;
        spriteRenderer.DrawPosOffSet = -origin;
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
            Collider.DrawRectangleNoSprite(collisionRectangle.Rectangle, Color.Black, spriteBatch);
        }
        base.Draw(spriteBatch);
    }
}