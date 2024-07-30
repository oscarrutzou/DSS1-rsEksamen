using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DoctorsDungeon.ComponentPattern.Particles
{
    public class Particle : Component, IParticle
    {
        public double Age { get; set; }
        public double MaxAge { get; set; }
        public Vector2 Velocity { get; set; }
        public float RotationVelocity { get; set; }
        public Vector2 Position
        {
            get { return GameObject.Transform.Position; }
            set { GameObject.Transform.Position = value; }
        }
        public Vector2 Scale
        {
            get { return GameObject.Transform.Scale; }
            set { GameObject.Transform.Scale = value; }
        }
        public double Alpha
        {
            get { return Color.A; }
            set
            {
                Color color = Color;
                color.A = (byte)(Math.Clamp(value, 0, 1) * 255);
            }
        }

        private Color nextColor;
        public Color Color
        {
            get 
            {
                if (spriteRenderer == null) return nextColor;
                return spriteRenderer.Color; 
            }
            set 
            { 
                if (spriteRenderer == null)
                {
                    nextColor = value;
                    return;
                }
                nextColor = value;
                spriteRenderer.Color = value; 
            }
        }

        public TextOnSprite TextOnSprite { get; set; } = new TextOnSprite()
        {
            Text = "Test"
        };

        private SpriteRenderer spriteRenderer;

        public Particle(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.Color = Color;

            spriteRenderer.TextOnSprite = TextOnSprite;
        }

        //public override void Draw(SpriteBatch spriteBatch)
        //{
        //    int dem = spriteRenderer.Sprite.Width * (int)Scale.X;
        //    Rectangle spriteRec = new Rectangle((int)Position.X, (int)Position.Y, dem, dem);
        //    Collider.DrawRectangleNoSprite(spriteRec, Color.HotPink, spriteBatch);
        //}
    }
}
