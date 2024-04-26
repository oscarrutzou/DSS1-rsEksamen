using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using BaseProject.GameManagement;

namespace BaseProject.CompositPattern
{
    public class Animation
    {
        public AnimNames Name { get; private set; }
        public Texture2D[] Sprites { get; private set; }
        public int FPS { get; private set; }
        public Action OnAnimationDone { get; set; }
        public bool ShouldPlayAnim { get; set; } = true;
        public bool UseSpriteSheet { get; set; }
        public int FrameDimensions { get; private set; }
        public Rectangle SourceRectangle { get; private set; }
        public Animation(AnimNames name, Texture2D[] sprites, int fPS)
        {
            Name = name;
            Sprites = sprites;
            FrameDimensions = sprites[0].Width; // Assumes that the frame dem are the same though the animation and the width and height are the same
            FPS = fPS;
        }

        public Animation(AnimNames name, Texture2D[] sprites, int fPS, Action action)
        {
            Name = name;
            Sprites = sprites;
            FPS = fPS;
            FrameDimensions = sprites[0].Width; // Assumes that the frame dem are the same though the animation and the width and height are the same
            OnAnimationDone = action;
        }

        public Animation(AnimNames name, Texture2D[] sprites, int fPS, int frameDem)
        {
            Name = name;
            Sprites = sprites;
            FPS = fPS;
            FrameDimensions = frameDem;
            SourceRectangle = new Rectangle(0, 0, frameDem, frameDem);
            UseSpriteSheet = true;
        }

        public Animation(AnimNames name, Texture2D[] sprites, int fPS, Action action, int frameDem)
        {
            Name = name;
            Sprites = sprites;
            FPS = fPS;
            OnAnimationDone = action;
            FrameDimensions = frameDem;
            SourceRectangle = new Rectangle(0, 0, frameDem, frameDem);
            UseSpriteSheet = true;
        }
    }
}
