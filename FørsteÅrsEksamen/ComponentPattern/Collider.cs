using DoctorsDungeon.CommandPattern;
using DoctorsDungeon.ComponentPattern.WorldObjects;
using DoctorsDungeon.GameManagement;
using DoctorsDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DoctorsDungeon.ComponentPattern;

// Oscar
public class Collider : Component
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Texture2D texture;
    public int StartCollisionWidth { get; private set; }
    public int StartCollisionHeight { get; private set; } //If not set, use the sprite width and height

    private Vector2 positionOffset;

    public Color DebugColor = Color.Red;
    public Color DebugColorRotated = Color.Azure;

    public bool CenterCollisionBox = true;
    public Rectangle CollisionBox
    {
        get
        {
            int width, height;
            Vector2 pos = GameObject.Transform.Position;
            if (animator != null && animator.CurrentAnimation != null)
            {
                width = StartCollisionWidth > 0 ? StartCollisionWidth : animator.CurrentAnimation.FrameDimensions;
                height = StartCollisionHeight > 0 ? StartCollisionHeight : animator.CurrentAnimation.FrameDimensions;
            }
            else
            {
                width = StartCollisionWidth > 0 ? StartCollisionWidth : spriteRenderer.Sprite.Width;
                height = StartCollisionHeight > 0 ? StartCollisionHeight : spriteRenderer.Sprite.Height;
            }

            width *= (int)GameObject.Transform.Scale.X;
            height *= (int)GameObject.Transform.Scale.Y;

            // Make the collisionBox to be offcenter, with top left as its origin point.
            if (!CenterCollisionBox)
            {
                pos.X += width / 2;
                pos.Y += height / 2;
            }

            return new Rectangle
                (
                    (int)((pos.X - positionOffset.X) - (width) / 2),
                    (int)((pos.Y - positionOffset.Y) - (height) / 2),
                    width,
                    height
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
        if (!InputHandler.Instance.DebugMode) return;
        //DrawRectangle(CollisionBox, DebugColor, spriteBatch);
        DrawRotatedRectangle(CollisionBox, GameObject.Transform.Rotation, DebugColor, spriteBatch);
    }

    /// <summary>
    /// Set custom collsionBox
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetCollisionBox(int width, int height)
    {
        StartCollisionWidth = width;
        StartCollisionHeight = height;
    }

    public void SetCollisionBox(int width, int height, Vector2 positionOffset)
    {
        StartCollisionWidth = width;
        StartCollisionHeight = height;
        this.positionOffset = positionOffset;
    }

    /// <summary>
    /// Resets the custom collision box, and offset if it has been set
    /// </summary>
    public void ResetCustomCollsionBox()
    {
        positionOffset = Vector2.Zero;
        StartCollisionHeight = 0;
        StartCollisionWidth = 0;
    }

    public bool Contains(Vector2 point)
    {
        // Rotate the point
        Vector2 pos = GameObject.Transform.Position;

        Vector2 center;
        if (CenterCollisionBox)
            center = pos + new Vector2(CollisionBox.Width / 2, CollisionBox.Height / 2);
        else
            center = pos;

        var rotatedPoint = RotatePoint(point, center, -GameObject.Transform.Rotation); // Negative rotation to undo rectangle rotation

        return rotatedPoint.X >= GameObject.Transform.Position.X && rotatedPoint.X <= GameObject.Transform.Position.X + CollisionBox.Width &&
               rotatedPoint.Y >= GameObject.Transform.Position.Y && rotatedPoint.Y <= GameObject.Transform.Position.Y + CollisionBox.Height;
    }

    private Vector2 RotatePoint(Vector2 point, Vector2 pivot, float angle)
    {
        var cosTheta = MathF.Cos(angle);
        var sinTheta = MathF.Sin(angle);

        var x = cosTheta * (point.X - pivot.X) - sinTheta * (point.Y - pivot.Y) + pivot.X;
        var y = sinTheta * (point.X - pivot.X) + cosTheta * (point.Y - pivot.Y) + pivot.Y;

        return new Vector2(x, y);
    }

    /// <summary>
    /// Draws a debug line around the rectangle
    /// </summary>
    /// <param name="collisionBox"></param>
    /// <param name="spriteBatch"></param>
    /// <param name="vectorOffSet"></param>
    public void DrawRectangle(Rectangle collisionBox, Color color, SpriteBatch spriteBatch)
    {
        Vector2 colBoxPos = new Vector2(collisionBox.X, collisionBox.Y);

        int thickness = 1;
        Rectangle topLine = new Rectangle((int)colBoxPos.X, (int)colBoxPos.Y, collisionBox.Width, thickness);
        Rectangle bottomLine = new Rectangle((int)colBoxPos.X, (int)colBoxPos.Y + collisionBox.Height, collisionBox.Width, thickness);
        Rectangle rightLine = new Rectangle((int)colBoxPos.X + collisionBox.Width, (int)colBoxPos.Y, thickness, collisionBox.Height);
        Rectangle leftLine = new Rectangle((int)colBoxPos.X, (int)colBoxPos.Y, thickness, collisionBox.Height);

        spriteBatch.Draw(texture, topLine, null, color, 0, Vector2.Zero, spriteRenderer.SpriteEffects, 1);
        spriteBatch.Draw(texture, bottomLine, null, color, 0, Vector2.Zero, spriteRenderer.SpriteEffects, 1);
        spriteBatch.Draw(texture, rightLine, null, color, 0, Vector2.Zero, spriteRenderer.SpriteEffects, 1);
        spriteBatch.Draw(texture, leftLine, null, color, 0, Vector2.Zero, spriteRenderer.SpriteEffects, 1);
    }

    public void DrawRotatedRectangle(Rectangle collisionBox, float rotation, Color color, SpriteBatch spriteBatch)
    {
        Vector2[] edges = GetRotatedCorners(collisionBox, rotation);

        DrawLine(spriteBatch, edges[0], edges[1], color);
        DrawLine(spriteBatch, edges[0], edges[2], color);
        DrawLine(spriteBatch, edges[3], edges[1], color);
        DrawLine(spriteBatch, edges[3], edges[2], color);
    }

    public Vector2[] GetRotatedCorners(Rectangle collisionBox, float rotation)
    {
        Vector2 colBoxPos = new(collisionBox.X, collisionBox.Y);
        int width = collisionBox.Width;
        int height = collisionBox.Height;

        Vector2 center;
        if (CenterCollisionBox)
            center = colBoxPos + new Vector2(width / 2, height / 2);
        else
            center = colBoxPos;

        var topLeft = RotatePoint(colBoxPos, center, rotation);
        var topRight = RotatePoint(colBoxPos + new Vector2(width, 0), center, rotation);
        var bottomLeft = RotatePoint(colBoxPos + new Vector2(0, height), center, rotation);
        var bottomRight = RotatePoint(colBoxPos + new Vector2(width, height), center, rotation);

        return new[] { topLeft, topRight, bottomLeft, bottomRight };
    }

    public static void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
    {
        var distance = Vector2.Distance(point1, point2);
        var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
        DrawLine(spriteBatch, point1, distance, angle, color, thickness);
    }

    public static void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
    {
        var origin = new Vector2(0f, 0.5f);
        var scale = new Vector2(length, thickness);

        //point += new Vector2(50, 50);
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], point, null, color, angle, origin, scale, SpriteEffects.None, 0);
    }

    public static void DrawRectangleNoSprite(Rectangle rectangle, Color color, SpriteBatch spriteBatch)
    {
        Vector2 colBoxPos = new Vector2(rectangle.X, rectangle.Y);

        int thickness = 1;
        Rectangle topLine = new Rectangle((int)colBoxPos.X, (int)colBoxPos.Y, rectangle.Width, thickness);
        Rectangle bottomLine = new Rectangle((int)colBoxPos.X, (int)colBoxPos.Y + rectangle.Height, rectangle.Width, thickness);
        Rectangle rightLine = new Rectangle((int)colBoxPos.X + rectangle.Width, (int)colBoxPos.Y, thickness, rectangle.Height);
        Rectangle leftLine = new Rectangle((int)colBoxPos.X, (int)colBoxPos.Y, thickness, rectangle.Height);

        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], topLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], bottomLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], rightLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], leftLine, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
    }
}