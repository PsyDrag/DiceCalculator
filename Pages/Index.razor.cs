﻿using DiceCalculator;

namespace WasmDiceCalculator.Pages
{
    public partial class Index
    {
        private string diceRollInput = "3d6";
        private string minMaxInput = "min:1 max:20";
        private MinMax diceRollOutput = null;
        private int? rollOutput = null;
        private bool useRollOutput = false;
        private DiceRoll minMaxOutput = null;

        private void CalculateDiceRoll()
        {
            var dr = Parser.ParseDiceRoll(diceRollInput);
            diceRollOutput = Calculator.CalculateDiceRoll(dr);
            useRollOutput = false;
        }

        private void RollDice()
        {
            var dr = Parser.ParseDiceRoll(diceRollInput);
            rollOutput = DiceRoller.RollDice(dr);
            useRollOutput = true;
        }

        private void CalculateMinMax()
        {
            var mm = Parser.ParseMinMax(minMaxInput);
            minMaxOutput = Calculator.CalculateMinMax(mm);
        }
    }
}
