﻿namespace DiceCalculator
{
    public class DiceRoll
    {
        public DiceRoll(int numberOfDice, int numberOfDieFaces, string operation)
            => (NumDice, NumDieFaces, Operation) = (numberOfDice, numberOfDieFaces, operation);

        public DiceRoll(int numberOfDice, int numberOfDieFaces, bool keepHigh, int numberOfDiceToKeep, string operation)
            => (NumDice, KeepHigh, NumDiceToKeep, NumDieFaces, Operation)
            = (numberOfDice, keepHigh, numberOfDiceToKeep, numberOfDieFaces, operation);

        public int NumDice { get; set; }
        public bool KeepHigh { get; set; }
        public int NumDiceToKeep { get; set; }
        public int NumDieFaces { get; set; }
        public string Operation { get; set; }

        public bool NeedToDropDice()
        {
            return NumDiceToKeep != 0 && NumDiceToKeep < NumDice;
        }
    }
}
