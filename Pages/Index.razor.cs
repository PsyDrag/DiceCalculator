using System;
using DiceCalculator;
using Microsoft.AspNetCore.Components;

namespace WasmDiceCalculator.Pages
{
    public partial class Index
    {
        private string diceExpressionInput = "3d6";
        private string minMaxInput = "min:1 max:20";
        private MinMax diceExpressionOutput = null;
        private Tuple<DiceRollResult[], int> rollOutput = null;
        private Tuple<DiceRollResult[], int>[] setOutput = null;
        private bool useRollOutput = false;
        private DiceExpression minMaxOutput = null;
        private string graphButtonText = "Show Graph";
        private bool shouldShowGraph = false;
        private bool isCalcButtonDisabled = false;

        private void CalculateDiceExpression()
        {
            var expr = Parser.ParseDiceExpression(diceExpressionInput);
            diceExpressionOutput = Calculator.CalculateDiceExpression(expr);
            useRollOutput = false;
        }

        private void RollDice()
        {
            if (!diceExpressionInput.Contains('['))
            {
                var expr = Parser.ParseDiceExpression(diceExpressionInput);
                rollOutput = DiceRoller.RollDice(expr);
                setOutput = null;
                useRollOutput = true;
            }
            else
            {
                var set = Parser.ParseDiceSet(diceExpressionInput);
                setOutput = DiceRoller.RollDiceSet(set);
                rollOutput = null;
                useRollOutput = true;
            }
        }

        private void CalculateMinMax()
        {
            var mm = Parser.ParseMinMax(minMaxInput);
            minMaxOutput = Calculator.CalculateMinMax(mm);
        }

        private void OnGraphButtonClick()
        {
            shouldShowGraph = !shouldShowGraph;
            graphButtonText = shouldShowGraph ? "Hide Graph" : "Show Graph";
        }

        private void DiceExpressionValueChanged(ChangeEventArgs args)
        {
            bool disableCalcButton = args.Value.ToString().IndexOfAny(new char[] { '[', ']' }) != -1;
            isCalcButtonDisabled = disableCalcButton;
        }
    }
}
