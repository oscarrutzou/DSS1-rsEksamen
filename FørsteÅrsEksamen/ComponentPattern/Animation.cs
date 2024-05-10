using FørsteÅrsEksamen.GameManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FørsteÅrsEksamen.ComponentPattern
{
    // Oscar
    public class Animation
    {
        public AnimNames Name { get; private set; }
        public Texture2D[] Sprites { get; private set; }
        public int AmountOfFrames { get; private set; }
        public Action OnAnimationDone { get; set; }
        public bool ShouldPlayAnim { get; set; } = true;
        public bool UseSpriteSheet { get; set; }
        public int FrameDimensions { get; private set; }
        public Rectangle SourceRectangle { get; private set; }

        public Animation(AnimNames name, Texture2D[] sprites, int framesNr)
        {
            Name = name;
            Sprites = sprites;
            FrameDimensions = sprites[0].Width; // Assumes that the frame dem are the same though the animation and the width and height are the same
            AmountOfFrames = framesNr;
        }

        public Animation(AnimNames name, Texture2D[] sprites, int framesNr, Action action)
        {
            Name = name;
            Sprites = sprites;
            AmountOfFrames = framesNr;
            FrameDimensions = sprites[0].Width; // Assumes that the frame dem are the same though the animation and the width and height are the same
            OnAnimationDone = action;
        }

        public Animation(AnimNames name, Texture2D[] sprites, int framesNr, int frameDem)
        {
            Name = name;
            Sprites = sprites;
            AmountOfFrames = framesNr;
            FrameDimensions = frameDem;
            SourceRectangle = new Rectangle(0, 0, frameDem, frameDem);
            UseSpriteSheet = true;
        }

        public Animation(AnimNames name, Texture2D[] sprites, int framesNr, Action action, int frameDem)
        {
            Name = name;
            Sprites = sprites;
            AmountOfFrames = framesNr;
            OnAnimationDone = action;
            FrameDimensions = frameDem;
            SourceRectangle = new Rectangle(0, 0, frameDem, frameDem);
            UseSpriteSheet = true;
        }
    }
}