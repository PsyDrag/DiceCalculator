using DiceCalculator;

namespace WasmDiceCalculator.Pages
{
    public partial class Index
    {
        private int currentCount = 0;
        private string diceRoll = "3d6";
        private string minMax = "";

        private void DoThingy()
        {
            currentCount++;

            var dr = Parser.ParseDiceRoll(diceRoll);
            var mm = Calculator.CalculateDiceRoll(dr);
            minMax = $"Min:{mm.Min} Max:{mm.Max}";
        }
    }
}
