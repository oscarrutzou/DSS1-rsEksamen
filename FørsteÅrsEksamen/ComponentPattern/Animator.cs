using ShamansDungeon.GameManagement;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ShamansDungeon.ComponentPattern;

// Oscar
public class Animator : Component
{
    #region Properties
    public Animation CurrentAnimation { get; private set; }
    public int MaxFrames { get; set; }
    public int CurrentIndex { get; private set; }

    private SpriteRenderer _spriteRenderer;
    private Dictionary<AnimNames, Animation> _animations = new Dictionary<AnimNames, Animation>();
    private bool _isLooping, _hasPlayedAnim;
    private double _timeElapsed, _frameDuration;

    public Animator(GameObject gameObject) : base(gameObject)
    {
    }

    #endregion Properties

    public override void Awake()
    {
        _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            throw new Exception($"No spriteRenderer on gameObject, and therefore its not possible to Animate");
    }

    public override void Update()
    {
        if (CurrentAnimation == null) return;

        //if (IsLooping && hasPlayedAnim) return; // Already have played the animation once, so it can stop.
        _timeElapsed += GameWorld.DeltaTime;

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
        if (_timeElapsed > _frameDuration && !_hasPlayedAnim)
        {
            //Set new frame
            _timeElapsed = 0;
            CurrentIndex = (CurrentIndex + 1) % CurrentAnimation.Sprites.Length;

            if (CurrentIndex == 0)
            {
                if (!_isLooping) _hasPlayedAnim = true; // Stops looping after playing once

                CurrentAnimation.OnAnimationDone?.Invoke();
            }
        }

        if (CurrentIndex < 0) CurrentIndex = 0;
        _spriteRenderer.Sprite = CurrentAnimation.Sprites[CurrentIndex];
    }

    private void UpdateSpriteSheet()
    {
        if (_timeElapsed > _frameDuration && !_hasPlayedAnim)
        {
            //Set new frame
            _timeElapsed = 0;
            CurrentIndex = (CurrentIndex + 1) % MaxFrames; //So it turns to 0 when it goes over maxframes

            if (CurrentIndex == 0)
            {
                if (!_isLooping) _hasPlayedAnim = true; // Stops looping after playing once

                CurrentAnimation.OnAnimationDone?.Invoke();
            }
        }

        _spriteRenderer.SourceRectangle.X = CurrentIndex * CurrentAnimation.FrameDimensions; // Only works with animation thats horizontal
    }

    private void AddAnimation(AnimNames name) => _animations.Add(name, GlobalAnimations.Animations[name]);

    /// <summary>
    /// <para>Updates params based on chosen Animation. Also resets the IsLopping to true</para>
    /// </summary>
    /// <param name="animationName"></param>
    /// <exception cref="Exception"></exception>
    public void PlayAnimation(AnimNames animationName)
    {
        if (!_animations.ContainsKey(animationName))
        {
            AddAnimation(animationName);
        }

        if (CurrentAnimation != null) // Reset previous animation
        {
            _timeElapsed = 0;
            CurrentIndex = 0;
            CurrentAnimation.OnAnimationDone = null; //Resets its commands
            _spriteRenderer.OriginOffSet = Vector2.Zero;
            _spriteRenderer.DrawPosOffSet = Vector2.Zero;
        }

        CurrentAnimation = _animations[animationName];
        
        // Reset spriterenderer
        _spriteRenderer.UsingAnimation = true; // This gets set to false if you have played a Animation, then want to use a normal sprite again

        //_spriteRenderer.ShouldDrawSprite = true;
        _spriteRenderer.Rotation = -1;

        _frameDuration = 1f / CurrentAnimation.FPS; //Sets how long each frame should be
        _isLooping = true; // Resets loop
        _hasPlayedAnim = false;

        if (CurrentAnimation.UseSpriteSheet)
        {
            _spriteRenderer.SourceRectangle = CurrentAnimation.SourceRectangle; // Use a sourcerectangle to only show the specific part of the animation
            _spriteRenderer.Sprite = CurrentAnimation.Sprites[0]; //Only one animation in the spritesheet
            MaxFrames = _spriteRenderer.Sprite.Width / CurrentAnimation.FrameDimensions; // Only works with animation thats horizontal
        }
        else
        {
            _spriteRenderer.Sprite = CurrentAnimation.Sprites[CurrentIndex];
            MaxFrames = CurrentAnimation.Sprites.Length;
        }
    }

    public void StopCurrentAnimationAtLastSprite()
    {
        if (CurrentAnimation == null) throw new Exception("Set animation before you can call this method");

        _isLooping = false; // Stop animation from looping
        CurrentAnimation.OnAnimationDone += () => 
        { 
            CurrentIndex = MaxFrames - 1; 
        }; ; // The action that gets called when the animation is done
    }
}