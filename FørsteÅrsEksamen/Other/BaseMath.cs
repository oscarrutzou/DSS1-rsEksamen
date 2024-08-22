using DoctorsDungeon.GameManagement;
using DoctorsDungeon.GameManagement.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DoctorsDungeon.Other;

// Oscar
public static class BaseMath
{
    #region Circle Drawing & Making
    public static void DrawCircle(SpriteBatch spriteBatch, Vector2 position, Vector2 size, int segmentCount, Color color, float thickness)
    {
        float angleIncrement = 360f / segmentCount;

        Vector2 point = GetPointOfCircle(position, size, angleIncrement, 0);
        for (int i = 1; i < segmentCount; i++)
        {
            Vector2 nextPoint = GetPointOfCircle(position, size, angleIncrement, i);
            DrawLine(spriteBatch, point, nextPoint, color, thickness);
            point = nextPoint;
        }

        DrawLine(spriteBatch, point, GetPointOfCircle(position, size, angleIncrement, 0), color, thickness);
    }
    public static Vector2[] CreateCircle(Vector2 position, Vector2 size, int segmentCount)
    {
        Vector2[] circlePoints = new Vector2[segmentCount];

        float angleIncrement = 360f / segmentCount;

        for (int i = 0; i < segmentCount; i++)
            circlePoints[i] = GetPointOfCircle(position, size, angleIncrement, i);

        return circlePoints;
    }

    static Vector2 GetPointOfCircle(Vector2 position, Vector2 size, float angleIncrement, int i)
    {
        float angle = i * angleIncrement;
        float x = position.X + size.X * (float)Math.Cos(Math.PI * angle / 180f);
        float y = position.Y + size.Y * (float)Math.Sin(Math.PI * angle / 180f);

        Vector2 point = new Vector2(x, y);
        return point;
    }
    public static void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float radian, Color color, float thickness, float depthLayer = 0)
        => DrawLine(spriteBatch, point, length, radian, color, thickness, .5f, depthLayer);
    public static void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness, float depthLayer = 0)
 => DrawLine(spriteBatch, point1, point2, color, thickness, .5f, depthLayer);
    private static void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness, float offset, float depthLayer = 0)
    {
        if (thickness <= 0)
            return;

        float distance = Vector2.Distance(point1, point2);
        float radian = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

        DrawLine(spriteBatch, point1, distance, radian, color, thickness, offset, depthLayer);
    }

    private static void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float radian, Color color, float thickness, float offset, float depthLayer = 0)
    {
        if (thickness <= 0)
            return;

        var sc = MathF.SinCos(radian);
        Vector2 a = new Vector2(sc.Cos, sc.Sin);
        Vector2 b = new Vector2(-sc.Sin, sc.Cos);
        OffsetLine(ref point, ref length, thickness, offset, a, b);

        spriteBatch.Draw(GlobalTextures.Textures[TextureNames.Pixel],
                         point,
                         null,
                         color,
                         radian,
                         Vector2.Zero,
                         new Vector2(length, thickness),
                         SpriteEffects.None,
                         depthLayer);
    }
    private static void OffsetLine(ref Vector2 point, ref float length, float thickness, float offset, Vector2 a, Vector2 b)
    {
        Vector2 aOffset = -thickness * offset * a;
        Vector2 bOffset = -thickness * offset * b;
        point += aOffset + bOffset;
        length += thickness * offset;
    }

#endregion

    public static Vector2 Rotate(Vector2 position, float rotation)
    {
        float cos = (float)Math.Cos(rotation);
        float sin = (float)Math.Sin(rotation);

        float newX = position.X * cos - position.Y * sin;
        float newY = position.X * sin + position.Y * cos;

        return new Vector2(newX, newY);
    }

    /// <summary>
    /// <para>A easing method that backs up a little them rams forward.</para>
    /// <para>From https://easings.net/</para>
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float EaseInBack(float x)
    {
        const float c1 = 1.7f; // Amount of time spent in the starting "backing up" part.
        const float c2 = c1 + 1;

        return c2 * x * x * x - c1 * x * x;
    }

    public static float EaseInOutBack(float x)
    {
        const float c1 = 1.7f;
        const float c2 = c1 * 1.525f;

        return x < 0.5f
            ? (float)(Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
            : (float)(Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }

    public static float EaseOutBack(float x)
    {
        const float c1 = 1.7f;
        const float c3 = c1 + 1;

        return 1 + c3 * (float)Math.Pow(x - 1, 3) + c1 * (float)Math.Pow(x - 1, 2);
    }

    public static float EaseOutExpo(float x)
    {
        return x == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * x);
    }

    /// <summary>
    /// A method that makes a smooth transition
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float EaseInOutQuad(float x)
    {
        return x < 0.5 ? 2 * x * x : 1 - MathF.Pow(-2 * x + 2, 2) / 2;
    }

    public static float EaseOutQuart(float x)
    {
        return 1 - MathF.Pow(1 - x, 4);
    }
    public static float EaseOutCubic(float x)
    {
        return 1 - MathF.Pow(1 - x, 3);
    }
    public static float EaseInOutQuint(float x)
    {
        return x < 0.5 ? 16 * x * x * x * x * x : 1 - MathF.Pow(-2 * x + 2, 5) / 2;
    }

    public static float Clamp(float a, float low, float high)
    {
        return Math.Max(low, Math.Min(a, high));
    }

    public static Color TransitionColor(Color startColor)
    {
        Scene crntScene = GameWorld.Instance.CurrentScene;

        if (crntScene.IsChangingScene)
            return Color.Lerp(startColor, Color.Transparent, (float)crntScene.TransitionProgress);
        else
            return startColor;
    }

    public static Vector2 SafeNormalize(Vector2 value)
    {
        float length = value.Length();
        if (length > 0)
        {
            float num = 1f / length;
            value.X *= num;
            value.Y *= num;
        }
        return value;
    }

    public static void SafeNormalize(ref Vector2 value, out Vector2 result)
    {
        float length = value.Length();
        if (length > 0)
        {
            float num = 1f / length;
            result.X = value.X * num;
            result.Y = value.Y * num;
        }
        else
        {
            result = Vector2.Zero; 
        }
    }
}