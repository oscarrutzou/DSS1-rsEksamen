using DoctorsDungeon.ComponentPattern.GUI;
using DoctorsDungeon.ComponentPattern.PlayerClasses;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;

namespace DoctorsDungeon.ComponentPattern;

// Remember with GUI text it dosen't use these layerdepth to draw the text and just uses 1.
public enum LayerDepth
{
    Default,
    WorldBackground,
    BackgroundDecoration,

    EnemyUnder,
    //EnemyUnderWeapon,

    Player,
    //PlayerWeapon,

    EnemyOver,
    //EnemyOverWeapon,

    DamageParticles,
    WorldForeground,

    Cells,

    UI,
    Button,
    Text,
    CollisionDebug,
    Cursor,
    Size, // So we just can take LayerDepth.
}

public class TextOnSprite: ICloneable
{
    public string Text { get; set; }
    public float TextScale = 1;
    public bool IsCentered = true;
    public Color TextColor = Color.Red;

    public object Clone()
    {
        return new TextOnSprite
        {
            Text = this.Text,
            TextScale = this.TextScale,
            IsCentered = this.IsCentered,
            TextColor = this.TextColor
        };
    }
}

// Oscar
public class SpriteRenderer : Component
{
    #region Properties

    public Texture2D Sprite { get; set; }
    public TextOnSprite TextOnSprite { get; set; }
    public Color Color { get; set; } = Color.White;
    public Color StartColor { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 OriginOffSet { get; set; }
    public Vector2 DrawPosOffSet { get; set; }
    public bool ShouldDrawSprite { get; set; } = true;
    public bool ShouldDrawText = true;
    public bool IsCentered = true;
    /// <summary>
    /// A rotation for the sprite only, not the GameObject itself
    /// </summary>
    public float Rotation
    {
        get
        {
            if (_rotation == -1)
            {
                return GameObject.Transform.Rotation;
            }
            return _rotation;
        }
        set
        {
            _rotation = value;
        }
    }
    private float _rotation = -1;
    public LayerDepth LayerName { get; private set; } = ComponentPattern.LayerDepth.Default;
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
    public float LayerDepth { get; private set; }
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

    // Maybe do something with the Y of the GameObject transform, and maybe each SpriteRendere have a list
    // of the layers that they can be behind or infront. So something that never should be behind the object, cant be
    
    /// <summary>
    /// How to change the layer that the sprites gets drawn on. Remember there are both the World Cam and UI Cam
    /// </summary>
    /// <param name="layerName"></param>
    /// <param name="offSet">Should be very small like 0.0001 to not mess the layers up</param>
    public void SetLayerDepth(LayerDepth layerName, float offSet = 0)
    {
        LayerName = layerName;
        LayerDepth = ((float)LayerName / (Enum.GetNames(typeof(LayerDepth)).Length)) + offSet;
    }
    public void SetLayerDepth(float layerDepth, float offSet = 0)
    {
        LayerDepth = layerDepth + offSet;
    }

    public static float GetLayerDepth(LayerDepth layerName, float offSet = 0)
    {
        return ((float)layerName / (Enum.GetNames(typeof(LayerDepth)).Length)) + offSet;
    }

    /// <summary>
    /// Also sets IsCentered to false, so the offset can be used
    /// </summary>
    /// <param name="offset"></param>
    public void SetOriginOffset(Vector2 offset)
    {
        IsCentered = false;
        OriginOffSet = offset;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (TextOnSprite != null)
        {
            DrawText(spriteBatch);
        }

        if (Sprite != null && ShouldDrawSprite)
        {
            DrawSprite(spriteBatch, Sprite, Vector2.Zero, LayerDepth);
        }
    }

    /// <summary>
    /// Draws the sprite the same place, as the normal sprite that is set with SetSprite. 
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="sprite"></param>
    /// <param name="layerDepth"></param>
    public void DrawSprite(SpriteBatch spriteBatch, Texture2D sprite, Vector2 posOffset, float layerDepth)
    {
        // Parse into a float from int, so we can get the centered origin
        Origin = IsCentered ? new Vector2((float)sprite.Width / 2, (float)sprite.Height / 2) : OriginOffSet;

        drawPos = GameObject.Transform.Position + posOffset;

        if (GameObject.Type != GameObjectTypes.Weapon)
        {
            Vector2 rotatedOffset = BaseMath.Rotate(DrawPosOffSet, Rotation);
            drawPos += rotatedOffset;
        }

        //Draws the sprite
        spriteBatch.Draw(sprite,
                         drawPos,
                         null,
                         Color,
                         Rotation,
                         Origin,
                         GameObject.Transform.Scale,
                         SpriteEffects,
                         layerDepth);
    }
    /// <summary>
    ///         

    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name=""></param>
    public static void DrawCenteredSprite(SpriteBatch spriteBatch, TextureNames textureName, Vector2 pos, Color color, LayerDepth layer, float scale = 4f, SpriteEffects spriteEffects = SpriteEffects.None, float rotation = 0f)
    {
        Texture2D sprite = GlobalTextures.Textures[textureName];
        Vector2 spriteCentered = new Vector2(sprite.Width / 2, sprite.Height / 2);

        spriteBatch.Draw(sprite, pos, null, color, rotation, spriteCentered, scale, spriteEffects, GetLayerDepth(layer));
    }

    private void DrawText(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
                    GlobalTextures.DefaultFont, 
                    TextOnSprite.Text, 
                    GameObject.Transform.Position, 
                    TextOnSprite.TextColor,
                    Rotation,
                    Vector2.Zero,
                    TextOnSprite.TextScale,
                    SpriteEffects.None,
                    LayerDepth);
    }
     
    public void SetSprite(TextureNames spriteName)
    {
        UsingAnimation = false;
        OriginOffSet = Vector2.Zero;
        DrawPosOffSet = Vector2.Zero;
        Sprite = GlobalTextures.Textures[spriteName];
        Rotation = -1;
    }
}