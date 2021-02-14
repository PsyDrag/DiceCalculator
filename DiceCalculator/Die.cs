namespace DiceCalculator
{
    public class Die
    {
        public Die(int diceAmount, int diceType)
        {
            TotalDiceAmount = diceAmount;
            DiceType = diceType;
        }

        public Die(int total, int keep, int diceType)
        {
            TotalDiceAmount = total;
            KeepAmount = keep;
            DiceType = diceType;
        }

        public int TotalDiceAmount { get; set; }
        public int KeepAmount { get; set; }
        public int DiceType { get; set; }
    }
}
