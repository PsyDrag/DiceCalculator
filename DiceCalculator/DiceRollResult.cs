using System;

namespace DiceCalculator
{
    public class DiceRollResult
    {
        public DiceRollResult(string op, Tuple<int, DieRollOutput>[] rolls, int sum)
            => (Operation, Rolls, Sum) = (op, rolls, sum);

        public string Operation { get; set; }
        public Tuple<int, DieRollOutput>[] Rolls { get; set; }
        public int Sum { get; set; }
    }

    [Flags]
    public enum DieRollOutput
    {
        Normal = 1,
        CritSuccess = 2,
        CritFailure = 4,
        Dropped = 8
    }
}
