using System;

namespace DoctorsDungeon.ComponentPattern.Particles
{
    public class Interval
    {
        private static Random rnd = new();
        private readonly double min;
        private readonly double max;

        public Interval(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

        public double GetValue()
        {
            return (rnd.NextDouble() * (max - min)) + min;
        }
    }
}
