using System.Collections.Generic;
using System.Diagnostics;

namespace DiceCalculator
{
    public class MinMax
    {
        public MinMax(int min, float avg, int max)
            => (Min, Avg, Max) = (min, avg, max);

        public MinMax(int min, float avg, int max, Dictionary<int, Calculations> calcs)
            => (Min, Avg, Max, MinMaxCalcs) = (min, avg, max, calcs);

        public int Min { get; internal set; }
        public float Avg { get; internal set; }
        public int Max { get; internal set; }

        public Dictionary<int, Calculations> MinMaxCalcs { get; internal set; }
    }

    [DebuggerDisplay("Freq: {Frequency}, Pct: {Percentage}")]
    public class Calculations
    {
        public Calculations(int freq, float pct)
            => (Frequency, Percentage) = (freq, pct);

        public int Frequency { get; internal set; }
        public float Percentage { get; internal set; }
    }
}