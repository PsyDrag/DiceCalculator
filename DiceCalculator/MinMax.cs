namespace DiceCalculator
{
    public class MinMax
    {
        public MinMax(int min, float avg, int max)
            => (Min, Avg, Max) = (min, avg, max);

        public int Min { get; internal set; }
        public float Avg { get; internal set; }
        public int Max { get; internal set; }
    }
}