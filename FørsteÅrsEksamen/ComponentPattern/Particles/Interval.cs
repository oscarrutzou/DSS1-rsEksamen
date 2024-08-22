using System;

namespace DoctorsDungeon.ComponentPattern.Particles
{
    public class Interval
    {
        private static Random _rnd = new();
        private readonly double _min;
        private readonly double _max;

        public Interval(double min, double max)
        {
            this._min = min;
            this._max = max;
        }

        public double GetValue()
        {
            return (_rnd.NextDouble() * (_max - _min)) + _min;
        }
    }
}
