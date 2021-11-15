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
        public Calculations(int value, int freq, float pct)
            => (Value, Frequency, Percentage) = (value, freq, pct);

        public int Value{ get; internal set; }
        public int Frequency { get; internal set; }
        public float Percentage { get; internal set; }
    }
}