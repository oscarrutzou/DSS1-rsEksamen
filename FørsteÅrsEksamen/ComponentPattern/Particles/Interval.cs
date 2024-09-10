using System;

namespace ShamansDungeon.ComponentPattern.Particles
{
    public class Interval
    {
        private static Random _rnd = new();
        public double Min { get; set; }
        public double Max { get; set; }

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
