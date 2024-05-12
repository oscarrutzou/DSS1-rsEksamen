using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FørsteÅrsEksamen.ComponentPattern
{
    // Remember with GUI text it dosen't use these layerdepth to draw the text and just uses 1.
    public enum LAYERDEPTH
    {
        Default,
        WorldBackground,
        EnemyUnderPlayer,
        Player,
        EnemyOverPlayer,
        WorldForeground,
        UI,
        Button,
        Text,
        CollisionDebug,
    }

    // Oscar
    public class SpriteRenderer : Component
    {
        #region Properties

        public Texture2D Sprite { get; set; }
        public bool ShouldDraw { get; set; } = true;

        public Color Color { get; set; } = Color.White;
        public Vector2 Origin { get; set; }
        public Vector2 OffSet { get; set; }
        public bool IsCentered = true;
        public LAYERDEPTH LayerName { get; private set; } = LAYERDEPTH.Default;
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
        private float LayerDepth;
        private Vector2 drawPos;

        // For the animation draw calls
        public Rectangle SourceRectangle;

        public bool UsingAnimation;
        private Animator animator;

        #endregion Properties

        public SpriteRenderer(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
        }

        public void SetLayerDepth(LAYERDEPTH layerName)
        {
            LayerName = layerName;
            LayerDepth = (float)LayerName / (Enum.GetNames(typeof(LAYERDEPTH)).Length - 1);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Sprite == null || !ShouldDraw) return;

            Origin = IsCentered ? new Vector2(Sprite.Width / 2, Sprite.Height / 2) : Vector2.Zero;

            drawPos = GameObject.Transform.Position;

            if (animator != null && animator.CurrentAnimation.UseSpriteSheet)
            {
                drawPos += new Vector2(animator.MaxFrames * animator.CurrentAnimation.FrameDimensions * GameObject.Transform.Scale.X / 2 - animator.CurrentAnimation.FrameDimensions * 2, 0);
            }

            drawPos += OffSet;

            //Draws the sprite, and if there is a sourcerectangle set, then it uses that.
            spriteBatch.Draw(Sprite, drawPos, SourceRectangle == Rectangle.Empty ? null : SourceRectangle, Color, GameObject.Transform.Rotation, Origin, GameObject.Transform.Scale, SpriteEffects, LayerDepth);
        }

        public void SetSprite(TextureNames spriteName)
        {
            UsingAnimation = false;
            OffSet = Vector2.Zero;
            Sprite = GlobalTextures.Textures[spriteName];
        }
    }
}