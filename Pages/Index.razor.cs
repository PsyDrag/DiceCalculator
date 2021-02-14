using System.Linq;
using DiceCalculator;

namespace WasmDiceCalculator.Pages
{
    public partial class Index
    {
        private string diceRollInput = "3d6";
        private string diceRollOutput = "";
        private string minMaxInput = "Min:1 Max:20";
        private string minMaxOutput = "";

        private void CalculateDiceRoll()
        {
            var dr = Parser.ParseDiceRoll(diceRollInput);
            var mm = Calculator.CalculateDiceRoll(dr);
            diceRollOutput = $"Output:\nMin:{mm.Min} Avg:{mm.Avg} Max:{mm.Max}";
        }

        private void CalculateMinMax()
        {
            var mm = Parser.ParseMinMax(minMaxInput);
            var dr = Calculator.CalculateMinMax(mm);

            var die = dr.Dice.First();
            minMaxOutput = $"Output:\n{die.TotalDiceAmount}d{die.DiceType}";

            var mod = dr.Modifiers.First();
            if (mod.Number != 0)
            {
                minMaxOutput += $" {mod.Operation} {mod.Number}";
            }
        }
    }
}
