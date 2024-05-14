using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.ComponentPattern.Weapons
{
    /// <summary>
    /// A class used to generate a collider that moves a rotation around its startPos
    /// </summary>
    internal class CollisionRectangle
    {
        public Rectangle Rectangle;
        public Vector2 StartPos;
    }
    // Only happen on attack. Also add hands. Remove it from the player and use 2 hands. 
    // The hands should be given and made before making the weapon, as a part of which hands we should use. 
    // Use the clenched hand for the one for the weapon and relaxed hand for the other. 
    // Make it easy to set how much it should lerp to and from. 
    // Make it wait for some time (need to be able to be set) before roating back. 

    // Maybe as a nice to have, look into how to make one of the less smooth path. So it e.g is fast in the beginning and slow at the end.
    // A really nice to have to so make a trail behind the weapon when it swings:D Would be fun to make
    internal abstract class Weapon : Component
    {
        public string weaponName;
        public int damage;

        internal int attackSpeed;

        internal SpriteRenderer spriteRenderer;
        internal float lerpFromTo = MathHelper.Pi;

        private float totalElapsedTime = 0.0f;
        private bool isRotatingBack = false;
        private List<CollisionRectangle> weaponColliders = new();

        protected Weapon(GameObject gameObject) : base(gameObject)
        {
            attackSpeed = 1; 
        }
        

        public override void Awake()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.IsCentered = false;

            SetStartColliders(new Vector2(7.5f, 38), 5, 5, 6, 4); // Gets set in each of the weapons insted of here.
        }

        /// <summary>
        /// To make colliders for the weapon.
        /// </summary>
        /// <param name="origin">How far the origin is from the top left corner. Should have a -0.5f in X to make it centered.</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="heightFromOriginToHandle">Height from the origin on the sprite to the end of the handle</param>
        /// <param name="amountOfColliders"></param>
        internal void SetStartColliders(Vector2 origin, int width, int height, int heightFromOriginToHandle, int amountOfColliders)
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

                weaponColliders.Add(new CollisionRectangle()
                {
                    Rectangle = MakeRec(pos, width, height, scale),
                    StartPos = pos
                });
            }
        }

        internal Rectangle MakeRec(Vector2 pos, int width, int height, Vector2 scale) => new Rectangle((int)pos.X, (int)pos.Y, width * (int)scale.X, (int)scale.Y * height);






        public void Attack()
        {
            
        }




        public override void Update(GameTime gameTime)
        {
            //Move a lot to attack method.
            totalElapsedTime += GameWorld.DeltaTime * attackSpeed; // To change the speed of the animation, change the attackspeed.
            if (totalElapsedTime >= 1f)
            {
                totalElapsedTime = 0f; // Reset totalElapsedTime
                isRotatingBack = !isRotatingBack; // Switch rotation direction
            }

            // Play with some other methods, for different weapons, to make them feel slow or fast https://easings.net/
            float easedTime; // maybe switch between them. 

            if (isRotatingBack)
            {
                // Rotate back to 0
                //easedTime = EaseInBack(totalElapsedTime); // Feels heavy
                easedTime = EaseInOutBack(totalElapsedTime); // Feels heavy
                GameObject.Transform.Rotation = MathHelper.Lerp(lerpFromTo, 0, easedTime);
            }
            else
            {
                // Rotate to PiOver2
                easedTime = EaseOutBack(totalElapsedTime); 
                GameObject.Transform.Rotation = MathHelper.Lerp(0, lerpFromTo, easedTime);
            }

            UpdateCollisionBoxesPos(GameObject.Transform.Rotation);
        }


        private void UpdateCollisionBoxesPos(float rotation)
        {
            foreach (CollisionRectangle collisionRectangle in weaponColliders)
            {
                Vector2 newPos = Rotate(collisionRectangle.StartPos, rotation);
                collisionRectangle.Rectangle.X = (int)newPos.X - collisionRectangle.Rectangle.Width / 2;
                collisionRectangle.Rectangle.Y = (int)newPos.Y - collisionRectangle.Rectangle.Height / 2;
            }
        }


        private Vector2 Rotate(Vector2 position, float rotation)
        {
            float cos = (float)Math.Cos(rotation);
            float sin = (float)Math.Sin(rotation);

            float newX = position.X * cos - position.Y * sin;
            float newY = position.X * sin + position.Y * cos;

            return new Vector2(newX, newY);
        }


        /// <summary>
        /// <para>A easing method that backs up a little them rams forward.</para>
        /// <para>From https://easings.net/</para>
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float EaseInBack(float x)
        {
            const float c1 = 1.7f; // Amount of time spent in the starting "backing up" part.
            const float c2 = c1 + 1;

            return c2 * x * x * x - c1 * x * x;
        }

        public float EaseInOutBack(float x)
        {
            const float c1 = 1.7f;
            const float c2 = c1 * 1.525f;

            return x < 0.5f
                ? (float)(Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
                : (float)(Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
        }

        public float EaseOutBack(float x)
        {
            const float c1 = 1.7f;
            const float c3 = c1 + 1;

            return 1 + c3 * (float)Math.Pow(x - 1, 3) + c1 * (float)Math.Pow(x - 1, 2);
        }

        public float EaseOutExpo(float x)
        {
            return x == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * x);
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (CollisionRectangle collisionRectangle in weaponColliders)
            {
                Collider.DrawRectangleNoSprite(collisionRectangle.Rectangle, Color.Black, spriteBatch);
            }


            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], GameObject.Transform.Position, null, Color.Red, GameObject.Transform.Rotation, Vector2.Zero, 10, SpriteEffects.None, 1);
            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], GameObject.Transform.Position, null, Color.Pink, GameObject.Transform.Rotation, Vector2.Zero, 10, SpriteEffects.None, 1);
        }
    }
}
