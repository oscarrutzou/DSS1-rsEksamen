using ShamansDungeon.CommandPattern;
using ShamansDungeon.ComponentPattern.WorldObjects;
using ShamansDungeon.GameManagement;
using ShamansDungeon.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ShamansDungeon.GameManagement.Scenes;
using System.Collections.Generic;

namespace ShamansDungeon.ComponentPattern;

public enum ColliderLayer
{
    Default,
    Player,
    Weapon,
    Enemy,
    Gui,
    BackPackIcon,
}

// Oscar
public class Collider : Component
{
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Texture2D _texture;
    public int StartCollisionWidth { get; private set; }
    public int StartCollisionHeight { get; private set; } //If not set, use the sprite width and height

    private Vector2 _positionOffset { get; set; }

    public Color DebugColor = Color.Red;
    public Color DebugColorRotated = Color.Azure;

    /// <summary>
    /// Centers both origin of collider and position.
    /// </summary>
    public bool CenterCollisionBox = true;
    public Rectangle CollisionBox
    {
        get
        {
            int width, height;
            Vector2 pos = GameObject.Transform.Position;
            if (_animator != null && _animator.CurrentAnimation != null)
            {
                width = StartCollisionWidth > 0 ? StartCollisionWidth : _animator.CurrentAnimation.FrameDimensions;
                height = StartCollisionHeight > 0 ? StartCollisionHeight : _animator.CurrentAnimation.FrameDimensions;
            }
            else
            {
                width = StartCollisionWidth > 0 ? StartCollisionWidth : _spriteRenderer.Sprite.Width;
                height = StartCollisionHeight > 0 ? StartCollisionHeight : _spriteRenderer.Sprite.Height;
            }

            width *= (int)GameObject.Transform.Scale.X;
            height *= (int)GameObject.Transform.Scale.Y;

            if (CenterCollisionBox)
            {
                pos.X -= width / 2;
                pos.Y -= height / 2;
            }
            return new Rectangle
                (
                    (int)(pos.X - _positionOffset.X),
                    (int)(pos.Y - _positionOffset.Y),
                    width,
                    height
                );
        }
    }
    public Vector2 LeftTopPosRectangle
    {
        get
        {
            Vector2 pos = GameObject.Transform.Position;
            if (CenterCollisionBox)
            {
                pos.X -= CollisionBox.Width / 2;
                pos.Y -= CollisionBox.Height / 2;
            }
            return pos;
        }
    }
    public ColliderLayer ColliderLayer { get; private set; }
    public List<ColliderLayer> LayersToCollideWith { get; private set; } 
    public Collider(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Start()
    {
        _animator = GameObject.GetComponent<Animator>();
        _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        _texture = GlobalTextures.Textures[TextureNames.Pixel];
    }
    public void SetColliderLayer(ColliderLayer layer)
    {
        ColliderLayer = layer;
        SceneData.Instance.ColliderMeshLists[layer].Add(GameObject);
    }
    public void SetColliderLayer(ColliderLayer layer, List<ColliderLayer> layersToCollideWith)
    {
        ColliderLayer = layer;
        SceneData.Instance.ColliderMeshLists[layer].Add(GameObject);
        LayersToCollideWith = layersToCollideWith;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!InputHandler.Instance.DebugMode) return;
        DrawRotatedRectangle(CollisionBox, 0f, DebugColor, spriteBatch);
        
        if (GameObject.Transform.Rotation != 0.0f) // So we dont have layer figthing
            DrawRotatedRectangle(CollisionBox, GameObject.Transform.Rotation, DebugColorRotated, spriteBatch);
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
        this._positionOffset = positionOffset;
    }

    /// <summary>
    /// Resets the custom collision box, and offset if it has been set
    /// </summary>
    public void ResetCustomCollsionBox()
    {
        _positionOffset = Vector2.Zero;
        StartCollisionHeight = 0;
        StartCollisionWidth = 0;
    }

    public static Vector2 RotatePoint(Vector2 point, Vector2 pivot, float angle)
    {
        var cosTheta = MathF.Cos(angle);
        var sinTheta = MathF.Sin(angle);

        var x = cosTheta * (point.X - pivot.X) - sinTheta * (point.Y - pivot.Y) + pivot.X;
        var y = sinTheta * (point.X - pivot.X) + cosTheta * (point.Y - pivot.Y) + pivot.Y;

        return new Vector2(x, y);
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
        int width = collisionBox.Width;
        int height = collisionBox.Height;

        Vector2 colBoxPos = new(collisionBox.X, collisionBox.Y);
        Vector2 center = CenterCollisionBox ? GameObject.Transform.Position : colBoxPos;

        var topLeft = RotatePoint(colBoxPos, center, rotation);
        var topRight = RotatePoint(colBoxPos + new Vector2(width, 0), center, rotation);
        var bottomLeft = RotatePoint(colBoxPos + new Vector2(0, height), center, rotation);
        var bottomRight = RotatePoint(colBoxPos + new Vector2(width, height), center, rotation);

        return new[] { topLeft, topRight, bottomLeft, bottomRight };
    }

    public bool Contains(Vector2 point)
    {
        // Calculate the center of the collision box 
        Vector2 center = CenterCollisionBox ? GameObject.Transform.Position : new Vector2(CollisionBox.X, CollisionBox.Y);

        Vector2 pos = new Vector2(CollisionBox.X, CollisionBox.Y);

        float halfWidth = CollisionBox.Width / 2f;
        float halfHeight = CollisionBox.Height / 2f;

        // Rotate the point
        var rotatedPoint = RotatePoint(point, center, -GameObject.Transform.Rotation); // Negative rotation to undo rectangle rotation
        bool withinXBounds;
        bool withinYBounds;

        
        if (CenterCollisionBox)
        {
            withinXBounds = MathF.Abs(rotatedPoint.X - center.X + _positionOffset.X) <= halfWidth;
            withinYBounds = MathF.Abs(rotatedPoint.Y - center.Y + _positionOffset.Y) <= halfHeight;
            
            // -60 point <= 48
            //float y = rotatedPoint.Y - center.Y;
            //if (rotatedPoint.Y <= 0)
            //{
            //    withinYBounds = y <= halfHeight;
            //}
            // 60 rotated point.
            // needs first to take the normal point, then rotate the point
        }
        else
        {
            withinXBounds = rotatedPoint.X >= center.X
                         && rotatedPoint.X <= center.X + CollisionBox.Width;

            withinYBounds = rotatedPoint.Y >= center.Y
                         && rotatedPoint.Y <= center.Y + CollisionBox.Height;
        }

        return withinXBounds && withinYBounds;
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
        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel], point, null, color, angle, origin, scale, SpriteEffects.None, 0.9f);
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