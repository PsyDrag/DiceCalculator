namespace DiceCalculator
{
    public class Die
    {
        public Die(int diceAmount, int diceType, string operation)
            => (TotalDiceAmount, DiceType, Operation) = (diceAmount, diceType, operation);

        public Die(int total, int diceType, bool keepHigh, int keep, string operation)
            => (TotalDiceAmount, KeepHigh, KeepAmount, DiceType, Operation) = (total, keepHigh, keep, diceType, operation);

        public int TotalDiceAmount { get; set; }
        public bool KeepHigh { get; set; }
        public int KeepAmount { get; set; }
        public int DiceType { get; set; }
        public string Operation { get; set; }
    }
}
