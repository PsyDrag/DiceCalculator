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

        public static void Print(string line, DiceRoll result)
        {
            string separator = new string('-', line.Length);
            Console.WriteLine($"\n  {line}\n  {separator}");

            var die = result.Dice.First();
            string lineToPrint = $"  {die.TotalDiceAmount}d{die.DiceType}";
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
            Console.WriteLine("Usage: [options]");
            Console.WriteLine();
            Console.WriteLine("Options:                            ");
            Console.WriteLine("  h|help                            Display help");
            Console.WriteLine("  e|exit                            Exit program");
            Console.WriteLine("  dr|diceroll [diceRollComponents]  You've been doing this for years so you ought to be able to");
            Console.WriteLine("  mm|minmax [minMaxComponents]      Must always include min and max");
            Console.WriteLine("  drex|drexamples                   Show examples of dice rolls");
            Console.WriteLine("  mmex|mmexamples                   Show examples of min-max values");
            Console.WriteLine();
            Console.WriteLine("Dice Roll Components:");
            Console.WriteLine("  d                                 Do I really need to explain this one?");
            Console.WriteLine("  kh                                Keep highest x number of dice rolls (e.g. 4kh3)");
            Console.WriteLine("  kl                                Keep lowest x number of dice rolls (e.g. 2kl1)");
            Console.WriteLine("  ac                                Show pass/fail % against an AC");
            Console.WriteLine("  dc                                Show pass/fail % against a DC");
            Console.WriteLine();
            Console.WriteLine("Min-Max Components:");
            Console.WriteLine("  min                               Minimum value");
            Console.WriteLine("  avg                               Average value");
            Console.WriteLine("  max                               Maximum value");
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void PrintDiceRollExamples()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Dice Roll Examples:");
            Console.WriteLine("  1d4 + 7              standard roll plus modifier");
            Console.WriteLine("  3d6                  no modifier");
            Console.WriteLine("  d20                  straight up");
            Console.WriteLine("  d6 * 2               multiply");
            Console.WriteLine("  d20 + 3d6 + 5        multiple dice rolls");
            Console.WriteLine("  d6 - 3 * 2           multiple modifiers (calculations are made from left to right)");
            Console.WriteLine("  4kh3d6               standard ability roll");
            Console.WriteLine("  2kl1d20              with disadvantage");
            Console.WriteLine("  6kl5d4 + 3kh2d6 + 2  why would you want to do this fancy shit?");
            Console.WriteLine("  d20 + 6 ac 18        attack roll against AC 18");
            Console.WriteLine("  d20 + 4 dc 15        check against DC 15");
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void PrintMinMaxExamples()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Min-Max Examples:");
            Console.WriteLine("  min:10 max:48          2d20 + 8 as a possible result");
            Console.WriteLine("  min:6 max:17           d12 + 5 as a possible result");
            //Console.WriteLine("  min:6 avg:11.5 max:17  d6 + 2d4 + 3 as a possible result");
            //Console.WriteLine("  min:6 avg:15 max:17    d12 + 5 (min and max are prioritized over avg)");
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
