namespace DiceCalculator
{
    public class MinMax
    {
        public MinMax(int min, float avg, int max)
        {
            Min = min;
            Avg = avg;
            Max = max;
        }

        public int Min { get; internal set; }
        public float Avg { get; internal set; }
        public int Max { get; internal set; }
    }
}