using DoctorsDungeon.GameManagement.Scenes;
using Microsoft.Xna.Framework;
using System;

namespace DoctorsDungeon.Other;

// Oscar
public static class BaseMath
{


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