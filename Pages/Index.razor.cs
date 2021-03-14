using DiceCalculator;

namespace WasmDiceCalculator.Pages
{
    public partial class Index
    {
        private string diceRollInput = "3d6";
        private string minMaxInput = "min:1 max:20";
        private MinMax diceRollOutput = null;
        private DiceRoll minMaxOutput = null;

        private void CalculateDiceRoll()
        {
            var dr = Parser.ParseDiceRoll(diceRollInput);
            diceRollOutput = Calculator.CalculateDiceRoll(dr);
        }

        private void CalculateMinMax()
        {
            var mm = Parser.ParseMinMax(minMaxInput);
            minMaxOutput = Calculator.CalculateMinMax(mm);
        }
    }
}
