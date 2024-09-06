using Microsoft.Xna.Framework;

namespace ShamansDungeon.ComponentPattern;

// Oscar
public class Transform
{
    public Vector2 Position { get; set; }
    public Point GridPosition { get; set; }
    public float Rotation { get; set; } = 0f;
    public Vector2 Scale { get; set; } = new Vector2(4, 4);

    public void Translate(Vector2 translation)
    {
        if (!float.IsNaN(translation.X) && !float.IsNaN(translation.Y))
        {
            Position += translation;
        }
    }
}