using FørsteÅrsEksamen.CommandPattern;
using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;

namespace FørsteÅrsEksamen.ComponentPattern.Weapons.MeleeWeapons
{
    public class Sword : MeleeWeapon
    {
        public Sword(GameObject gameObject) : base(gameObject)
        {
        }

        public Sword(GameObject gameObject, bool enemyWeapon) : base(gameObject, enemyWeapon)
        {
        }
        public override void Start()
        {
            AttackSoundNames = new SoundNames[]
            {
                SoundNames.SwipeSlow1,
            };
            spriteRenderer.SetSprite(TextureNames.WoodSword);
            SetStartColliders(new Vector2(7.5f, 38), 5, 5, 6, 4); // Gets set in each of the weapons insted of here.
        }

        public override void StartAttack()
        {
            if (Attacking) return;

            Attacking = true;
            TotalElapsedTime = 0f;

            Vector2 mouseInUI = InputHandler.Instance.MouseOnUI;

            // Set the direction of the animation. Change this part.
            if (mouseInUI.X > 0f) // Right
            {
                // -Y op
                // +Y ned
                if (mouseInUI.Y > 0f) // Down
                {
                    //start angle
                    TotalLerp = MathHelper.Pi + MathHelper.PiOver2;
                }
                else // Up
                {
                    //start angle
                    TotalLerp = MathHelper.Pi;
                }
            }
            else
            {
                // Y > 0f
                if (mouseInUI.Y > 0f) // Down
                {
                    //start angle
                    TotalLerp = -(MathHelper.Pi + MathHelper.PiOver2);
                }
                else // Up
                {
                    //start angle
                    TotalLerp = -LerpFromTo;
                }
            }
        }
    }
}