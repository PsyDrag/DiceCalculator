using System;
using System.Linq;

namespace DiceCalculator
{
    public static class Printer
    {
        public static void Print(string line, MinMax result)
        {
            string separator = new string('-', line.Length);
            Console.WriteLine($"\n  {line}\n  {separator}" + "\n{0}\n{1}\n{2}\n",
                $"  Min: {result.Min}",
                $"  Avg: {result.Avg}",
                $"  Max: {result.Max}");
        }

        public static void Print(string line, DiceExpression result)
        {
            string separator = new string('-', line.Length);
            Console.WriteLine($"\n  {line}\n  {separator}");

            var diceRoll = result.DiceRolls.First();
            string lineToPrint = $"  {diceRoll.NumDice}d{diceRoll.NumDieFaces}";
            var mod = result.Modifiers.First();
            if (mod.Number != 0)
            {
                lineToPrint += $" {mod.Operation} {mod.Number}";
            }
            lineToPrint += "\n";
            Console.WriteLine(lineToPrint);
        }

        public static void PrintError(string error)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(error);
            Console.ForegroundColor = temp;
        }

        public static void PrintHelp()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(Constants.Help);
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void PrintDiceExpressionExamples()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(Constants.DiceExpressionExamples);
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void PrintMinMaxExamples()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(Constants.MinMaxExamples);
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
