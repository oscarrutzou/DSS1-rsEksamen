using DoctorsDungeon.GameManagement;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DoctorsDungeon.ComponentPattern;

// Oscar
public class Animator : Component
{
    #region Properties

    private SpriteRenderer spriteRenderer;
    private Dictionary<AnimNames, Animation> animations = new Dictionary<AnimNames, Animation>();
    public Animation CurrentAnimation { get; private set; }

    private bool isLooping, hasPlayedAnim;
    public int CurrentIndex { get; private set; }
    public int MaxFrames { get; set; }
    private double timeElapsed, frameDuration;

    public Animator(GameObject gameObject) : base(gameObject)
    {
    }

    #endregion Properties

    public override void Start()
    {
        spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            throw new Exception($"No spriteRenderer on gameObject, and therefore its not possible to Animate");
    }

    public override void Update()
    {
        if (CurrentAnimation == null) return;

        //if (IsLooping && hasPlayedAnim) return; // Already have played the animation once, so it can stop.
        timeElapsed += GameWorld.DeltaTime;

        if (CurrentAnimation.UseSpriteSheet)
        {
            UpdateSpriteSheet();
        }
        else
        {
            UpdateIndividualFrames();
        }
    }

    private void UpdateIndividualFrames()
    {
        if (timeElapsed > frameDuration && !hasPlayedAnim)
        {
            //Set new frame
            timeElapsed = 0;
            CurrentIndex = (CurrentIndex + 1) % CurrentAnimation.Sprites.Length;

            if (CurrentIndex == 0)
            {
                if (!isLooping) hasPlayedAnim = true; // Stops looping after playing once

                CurrentAnimation.OnAnimationDone?.Invoke();
            }
        }

        if (CurrentIndex < 0) CurrentIndex = 0;
        spriteRenderer.Sprite = CurrentAnimation.Sprites[CurrentIndex];
    }

    private void UpdateSpriteSheet()
    {
        if (timeElapsed > frameDuration && !hasPlayedAnim)
        {
            //Set new frame
            timeElapsed = 0;
            CurrentIndex = (CurrentIndex + 1) % MaxFrames; //So it turns to 0 when it goes over maxframes

            if (CurrentIndex == 0)
            {
                if (!isLooping) hasPlayedAnim = true; // Stops looping after playing once

                CurrentAnimation.OnAnimationDone?.Invoke();
            }
        }

        spriteRenderer.SourceRectangle.X = CurrentIndex * CurrentAnimation.FrameDimensions; // Only works with animation thats horizontal
    }

    private void AddAnimation(AnimNames name) => animations.Add(name, GlobalAnimations.Animations[name]);

    /// <summary>
    /// <para>Updates params based on chosen Animation. Also resets the IsLopping to true</para>
    /// </summary>
    /// <param name="animationName"></param>
    /// <exception cref="Exception"></exception>
    public void PlayAnimation(AnimNames animationName)
    {
        if (!animations.ContainsKey(animationName))
        {
            AddAnimation(animationName);
        }

        if (CurrentAnimation != null) // Reset previous animation
        {
            CurrentIndex = 0;
            CurrentAnimation.OnAnimationDone = null; //Resets its commands
            spriteRenderer.OriginOffSet = Vector2.Zero;
            spriteRenderer.DrawPosOffSet = Vector2.Zero;
        }

        CurrentAnimation = animations[animationName];
        
        // Reset spriterenderer
        spriteRenderer.UsingAnimation = true; // This gets set to false if you have played a Animation, then want to use a normal sprite again


        //spriteRenderer.IsCentered = true;


        spriteRenderer.ShouldDrawSprite = true;
        spriteRenderer.Rotation = -1;

        frameDuration = 1f / CurrentAnimation.FPS; //Sets how long each frame should be
        isLooping = true; // Resets loop

        if (CurrentAnimation.UseSpriteSheet)
        {
            spriteRenderer.SourceRectangle = CurrentAnimation.SourceRectangle; // Use a sourcerectangle to only show the specific part of the animation
            spriteRenderer.Sprite = CurrentAnimation.Sprites[0]; //Only one animation in the spritesheet
            MaxFrames = spriteRenderer.Sprite.Width / CurrentAnimation.FrameDimensions; // Only works with animation thats horizontal
        }
        else
        {
            spriteRenderer.Sprite = CurrentAnimation.Sprites[CurrentIndex];
            MaxFrames = CurrentAnimation.Sprites.Length;
        }
    }

    public void StopCurrentAnimationAtLastSprite()
    {
        if (CurrentAnimation == null) throw new Exception("Set animation before you can call this method");

        isLooping = false; // Stop animation from looping
        CurrentAnimation.OnAnimationDone += () => 
        { 
            CurrentIndex = MaxFrames - 1; 
        }; ; // The action that gets called when the animation is done
    }
}