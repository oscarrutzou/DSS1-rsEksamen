using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FørsteÅrsEksamen.ComponentPattern.Weapons
{
    internal abstract class Weapon : Component
    {
        public string weaponName;
        public int damage;

        internal int range;
        internal int attackSpeed;
        internal float velocity, targetVelocity;

        private SpriteRenderer spriteRenderer;

        protected Weapon(GameObject gameObject) : base(gameObject)
        {
            attackSpeed = 10; 
            
        }
        
        private Vector2 spriteOffset;
        private List<Rectangle> weaponColliders = new();

        public override void Awake()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.IsCentered = false;
            spriteOffset = new(spriteRenderer.Sprite.Width / 2, spriteRenderer.Sprite.Height - 5);
            //this.GameObject.Transform.Position += new Vector2(0, spriteOffset.Y);
            //spriteOffset = new(spriteRenderer.Sprite.Width / 2, 0);

            spriteRenderer.OffSet = spriteOffset;

            AddWeaponColliders();
        }

        internal virtual void AddWeaponColliders()
        {
            Vector2 pos = GameObject.Transform.Position;
            weaponColliders.Add(MakeRec(pos, 20, 20));
        }

        internal Rectangle MakeRec(Vector2 pos, int width, int height) => new Rectangle((int)pos.X, (int)pos.Y, width, height);

        public void Attack()
        {
            
        }

        private float totalElapsedTime = 0.0f;
        private bool isRotatingBack = false;
        public override void Update(GameTime gameTime)
        {
            totalElapsedTime += GameWorld.DeltaTime;

            if (totalElapsedTime >= 1f)
            {
                totalElapsedTime = 0f; // Nulstil totalElapsedTime
                //isRotatingBack = !isRotatingBack; // Skift rotationsretning
            }

            if (isRotatingBack)
            {
                // Roter tilbage til 0
                GameObject.Transform.Rotation = MathHelper.Lerp(MathHelper.PiOver2, 0, totalElapsedTime);
            }
            else
            {
                // Roter til PiOver2
                GameObject.Transform.Rotation = MathHelper.Lerp(0, MathHelper.PiOver2, totalElapsedTime);
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Rectangle rectangle in weaponColliders)
            {
                Collider.DrawRectangleNoSprite(rectangle, Color.Black, spriteBatch);
            }

            spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], GameObject.Transform.Position + spriteOffset, null, Color.Pink, 0f, Vector2.Zero, 10, SpriteEffects.None, 1);
        }
    }
}
