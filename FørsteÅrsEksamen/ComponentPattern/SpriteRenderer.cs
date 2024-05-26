using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DoctorsDungeon.ComponentPattern
{
    // Remember with GUI text it dosen't use these layerdepth to draw the text and just uses 1.
    public enum LayerDepth
    {
        Default,
        WorldBackground,
        BackgroundDecoration,

        EnemyUnder,
        EnemyUnderWeapon,
        
        Player,
        PlayerWeapon,

        EnemyOver,
        EnemyOverWeapon,
        
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

        public Color Color { get; set; } = Color.White;
        public Vector2 Origin { get; set; }
        public Vector2 OriginOffSet { get; set; }
        public Vector2 DrawPosOffSet { get; set; }
        public bool ShouldDraw = true;
        public bool IsCentered = true;
        public LayerDepth LayerName { get; private set; } = ComponentPattern.LayerDepth.Default;
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

        public void SetLayerDepth(LayerDepth layerName)
        {
            LayerName = layerName;
            LayerDepth = (float)LayerName / (Enum.GetNames(typeof(LayerDepth)).Length - 1);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Sprite == null || !ShouldDraw) return;

            Origin = IsCentered ? new Vector2(Sprite.Width / 2, Sprite.Height / 2) : OriginOffSet;

            drawPos = GameObject.Transform.Position;

            if (animator != null && animator.CurrentAnimation != null && animator.CurrentAnimation.UseSpriteSheet)
            {
                drawPos += new Vector2(animator.MaxFrames * animator.CurrentAnimation.FrameDimensions * GameObject.Transform.Scale.X / 2 - animator.CurrentAnimation.FrameDimensions * 2, 0);
            }

            drawPos += OriginOffSet + DrawPosOffSet;

            //Draws the sprite, and if there is a sourcerectangle set, then it uses that.
            spriteBatch.Draw(Sprite, drawPos, SourceRectangle == Rectangle.Empty ? null : SourceRectangle, Color, GameObject.Transform.Rotation, Origin, GameObject.Transform.Scale, SpriteEffects, LayerDepth);
        }

        public void SetSprite(TextureNames spriteName)
        {
            UsingAnimation = false;
            OriginOffSet = Vector2.Zero;
            Sprite = GlobalTextures.Textures[spriteName];
        }
    }
}