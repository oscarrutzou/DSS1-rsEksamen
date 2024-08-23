using System;

namespace DoctorsDungeon.ComponentPattern.Particles
{
    public class Interval
    {
        private static Random _rnd = new();
        public double Min;
        public double Max;

        public Interval(double min, double max)
        {
            this.Min = min;
            this.Max = max;
        }

        public double GetValue()
        {
            return (_rnd.NextDouble() * (Max - Min)) + Min;
        }
    }
}
