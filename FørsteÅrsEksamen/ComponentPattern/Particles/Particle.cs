using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DoctorsDungeon.ComponentPattern.Particles.Emitter;

namespace DoctorsDungeon.ComponentPattern.Particles
{
    public class Particle : Component, IParticle
    {
        public double Age { get; set; }
        public double MaxAge { get; set; }
        public Vector2 Velocity { get; set; }
        public float RotationVelocity { get; set; }
        public Vector2 Scale
        {
            get { return GameObject.Transform.Scale; }
            set { GameObject.Transform.Scale = value; }
        }
        public double Alpha
        {
            get { return Color.A; }
            set
            {
                Color color = Color;
                color.A = (byte)(Math.Clamp(value, 0, 1) * 255);
            }
        }

        public Color Color
        {
            get { return spriteRenderer.Color; }
            set { spriteRenderer.Color = value; }
        }
        private SpriteRenderer spriteRenderer;

        public Particle(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.Color = Color;
        }
    
    }
}
