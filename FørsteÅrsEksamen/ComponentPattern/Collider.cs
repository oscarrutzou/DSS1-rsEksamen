using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FørsteÅrsEksamen.ComponentPattern
{
    // Oscar
    public class Collider : Component
    {
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Texture2D texture;
        private int collisionWidth, collisionHeight; //If not set, use the sprite width and height
        private Vector2 offset;

        public Rectangle CollisionBox
        {
            get
            {
                int width, height;

                if (animator != null)
                {
                    width = collisionWidth > 0 ? collisionWidth : animator.CurrentAnimation.FrameDimensions;
                    height = collisionHeight > 0 ? collisionHeight : animator.CurrentAnimation.FrameDimensions;
                }
                else
                {
                    width = collisionWidth > 0 ? collisionWidth : spriteRenderer.Sprite.Width;
                    height = collisionHeight > 0 ? collisionHeight : spriteRenderer.Sprite.Height;
                }

                return new Rectangle
                    (
                        (int)(GameObject.Transform.Position.X - (width * GameObject.Transform.Scale.X * GameWorld.Instance.WorldCam.zoom) / 2),
                        (int)(GameObject.Transform.Position.Y - (height * GameObject.Transform.Scale.Y * GameWorld.Instance.WorldCam.zoom) / 2),
                        (int)(width * GameObject.Transform.Scale.X * GameWorld.Instance.WorldCam.zoom),
                        (int)(height * GameObject.Transform.Scale.Y * GameWorld.Instance.WorldCam.zoom)
                    );
            }
        }

        public Collider(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            texture = GlobalTextures.Textures[TextureNames.Pixel];
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawRectangle(CollisionBox, spriteBatch, offset);
        }

        /// <summary>
        /// Set custom collsionBox
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetCollisionBox(int width, int height)
        {
            collisionWidth = width;
            collisionHeight = height;
        }

        public void SetCollisionBox(int width, int height, Vector2 offset)
        {
            collisionWidth = width;
            collisionHeight = height;
            this.offset = offset;
        }

        /// <summary>
        /// Resets the custom collision box, and offset if it has been set
        /// </summary>
        public void ResetCustomCollsionBox()
        {
            offset = Vector2.Zero;
            collisionHeight = 0;
            collisionWidth = 0;
        }

        /// <summary>
        /// Draws a debug line around the rectangle. Remember that it dosent show if the zoom is more that default.
        /// </summary>
        /// <param name="collisionBox"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="vectorOffSet"></param>
        private void DrawRectangle(Rectangle collisionBox, SpriteBatch spriteBatch, Vector2 vectorOffSet)
        {
            Vector2 colBoxPos = new Vector2(collisionBox.X, collisionBox.Y) + vectorOffSet;

            int thickness = Math.Max(1, (int)GameWorld.Instance.WorldCam.zoom);
            Rectangle topLine = new Rectangle((int)colBoxPos.X, (int)colBoxPos.Y, collisionBox.Width, thickness);
            Rectangle bottomLine = new Rectangle((int)colBoxPos.X, (int)colBoxPos.Y + collisionBox.Height, collisionBox.Width, thickness);
            Rectangle rightLine = new Rectangle((int)colBoxPos.X + collisionBox.Width, (int)colBoxPos.Y, thickness, collisionBox.Height);
            Rectangle leftLine = new Rectangle((int)colBoxPos.X, (int)colBoxPos.Y, thickness, collisionBox.Height);

            spriteBatch.Draw(texture, topLine, null, Color.Red, 0, Vector2.Zero, spriteRenderer.SpriteEffects, 1);
            spriteBatch.Draw(texture, bottomLine, null, Color.Red, 0, Vector2.Zero, spriteRenderer.SpriteEffects, 1);
            spriteBatch.Draw(texture, rightLine, null, Color.Red, 0, Vector2.Zero, spriteRenderer.SpriteEffects, 1);
            spriteBatch.Draw(texture, leftLine, null, Color.Red, 0, Vector2.Zero, spriteRenderer.SpriteEffects, 1);
        }
    }
}