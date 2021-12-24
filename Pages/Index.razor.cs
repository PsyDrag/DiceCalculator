using System;
using DiceCalculator;

namespace WasmDiceCalculator.Pages
{
    public partial class Index
    {
        private string diceExpressionInput = "3d6";
        private string minMaxInput = "min:1 max:20";
        private MinMax diceExpressionOutput = null;
        private Tuple<DiceRollResult[], int> rollOutput = null;
        private bool useRollOutput = false;
        private DiceExpression minMaxOutput = null;
        private string graphButtonText = "Show Graph";
        private bool shouldShowGraph = false;

        private void CalculateDiceRoll()
        {
            var expr = Parser.ParseDiceExpression(diceExpressionInput);
            diceExpressionOutput = Calculator.CalculateDiceExpression(expr);
            useRollOutput = false;
        }

        private void RollDice()
        {
            var expr = Parser.ParseDiceExpression(diceExpressionInput);
            rollOutput = DiceRoller.RollDice(expr);
            useRollOutput = true;
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
    }
}
