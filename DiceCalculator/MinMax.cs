using System.Collections.Generic;
using System.Diagnostics;

namespace DiceCalculator
{
    [DebuggerDisplay("Min: {Min}, Avg: {Avg}, Max: {Max}")]
    public class MinMax
    {
        public MinMax(int min, float avg, int max)
            => (Min, Avg, Max) = (min, avg, max);

        public MinMax(int min, float avg, int max, IList<Calculations> calcs)
            => (Min, Avg, Max, MinMaxCalcs) = (min, avg, max, calcs);

        public int Min { get; internal set; }
        public float Avg { get; internal set; }
        public int Max { get; internal set; }

        public IList<Calculations> MinMaxCalcs { get; internal set; }
    }

    [DebuggerDisplay("Value: {Value}, Freq: {Frequency}, Pct: {Percentage}")]
    public class Calculations
    {
        public Calculations(int count, long freq, float pct)
            => (Count, Frequency, Percentage) = (count, freq, pct);

        public int Count{ get; internal set; }
        public long Frequency { get; internal set; }
        public float Percentage { get; internal set; }
    }
}