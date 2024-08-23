using Microsoft.Xna.Framework;


namespace DoctorsDungeon.ComponentPattern.Particles
{
    public interface IParticle
    {
        public double Age { get; set; }
        public double MaxAge { get; set; }
        //public Vector2 Velocity { get; set; }
        public Vector3 VelocityZ { get; set; }
        public float RotationVelocity { get; set; }
        public Vector2 Position { get; set; }
        public Vector3 PositionZ { get; set; }
        public Vector2 Scale { get; set; }
        public double Alpha { get; set; } 
        public Color Color { get; set; }
        public TextOnSprite TextOnSprite { get; set; }
    }
}
