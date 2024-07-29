using Microsoft.Xna.Framework;
using System;

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public class AlphaBirthModifier : BirthModifier
    {
        private readonly double[] alphas;
        private Random rnd = new();

        public AlphaBirthModifier(params double[] alphas)
        {
            this.alphas = alphas;
        }

        public override void Execute(Emitter e, GameObject go, IParticle p)
        {
            p.Alpha = alphas[rnd.Next(alphas.Length)];
        }
    }
}
