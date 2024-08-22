using Microsoft.Xna.Framework;
using System;

namespace DoctorsDungeon.ComponentPattern.Particles.BirthModifiers
{
    public class AlphaBirthModifier : BirthModifier
    {
        private readonly double[] _alphas;
        private Random _rnd = new();

        public AlphaBirthModifier(params double[] alphas)
        {
            this._alphas = alphas;
        }

        public override void Execute(Emitter e, GameObject go, IParticle p)
        {
            p.Alpha = _alphas[_rnd.Next(_alphas.Length)];
        }
    }
}
