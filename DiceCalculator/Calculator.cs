using System;
using System.Linq;

namespace DiceCalculator
{
    public static class Calculator
    {
        public static MinMax CalculateDiceRoll(DiceRoll diceRoll)
        {
            var isKeepRoll = diceRoll.Dice.Any(d => d.KeepAmount != 0);
            if (isKeepRoll)
            {
                foreach (var die in diceRoll.Dice)
                {
                    if (die.TotalDiceAmount - die.KeepAmount != 1)
                    {
                        Printer.PrintError("This kind of dice roll is not yet implemented");
                        return new MinMax(0, 0, 0);
                    }
                }
            }

            CalculateMinMax(diceRoll, out var min, out var max);

            float avg;
            if (isKeepRoll)
            {
                //if 'kh' and total dice rolled - dice kept = 1 then where,
                //'n'    = n-sided die,
                //'tdr'  = total dice rolled
                //'navg' = avg on 1 n-sided die
                //the formula is ((n^tdr * tdr * navg) - (1^tdr + ... + n^tdr))
                //                                  (n^tdr)
                //
                //TODO: do more research to see how this formula changes when total dice rolled - dice kept > 1
                //TODO: do more research to figure out what the formula for 'kl' is
                var die = diceRoll.Dice.First();
                var n = die.DiceType;
                var tdr = die.TotalDiceAmount;
                var navg = (float)(n / 2 + 0.5);
                var ntdr = (int)Math.Pow(n, tdr);
                var summation = 1;
                for (int i = 2; i < n; i++)
                {
                    summation += (int)Math.Pow(i, tdr);
                }
                summation += ntdr;
                avg = (float)Math.Round(((ntdr * tdr * navg) - summation) / ntdr, 3);
                avg = AddModifiers(diceRoll, avg);
            }
            else
            {
                // mid-range
                avg = ((float)max + min) / 2;
            }

            return new MinMax(min, avg, max);
        }

        private static void CalculateMinMax(DiceRoll diceRoll, out int min, out int max)
        {
            min = 0;
            foreach (var die in diceRoll.Dice)
            {
                min += die.KeepAmount == 0 ? die.TotalDiceAmount : die.KeepAmount;
            }

            max = 0;
            foreach (var die in diceRoll.Dice)
            {
                max += (die.KeepAmount == 0 ? die.TotalDiceAmount : die.KeepAmount) * die.DiceType;
            }

            min = (int)AddModifiers(diceRoll, min);
            max = (int)AddModifiers(diceRoll, max);
        }

        private static float AddModifiers(DiceRoll diceRoll, float num)
        {
            foreach (var mod in diceRoll.Modifiers)
            {
                switch (mod.Operation)
                {
                    case "+":
                        num += mod.Number;
                        break;
                    case "-":
                        num -= mod.Number;
                        break;
                    case "*":
                        num *= mod.Number;
                        break;
                    case "/":
                        num /= mod.Number;
                        break;
                    default:
                        break;
                }
            }
            return num;
        }

        private static readonly int[] diceTypes = new int[] { 4, 6, 8, 10, 12, 20, 100 };
        public static DiceRoll CalculateMinMax(MinMax minMax)
        {
            if (minMax.Avg != 0.0)
            {
                Printer.PrintError("Calculating based off of avg is not yet implemented");
                return new DiceRoll(null, null);
            }

            if (minMax.Min == 1)
            {
                return new DiceRoll(new[] { new Die(1, minMax.Max) }, new[] { new Modifier("+", 0) });
            }

            int diceAmount;
            int diceType;
            int mod;
            for (int i = 1; i <= minMax.Min; i++)
            {
                mod = minMax.Min - i;
                var newMax = minMax.Max - mod;
                if (newMax % i == 0 && diceTypes.Contains(newMax / i))
                {
                    diceAmount = i;
                    diceType = newMax / i;
                    return new DiceRoll(new[] { new Die(diceAmount, diceType) }, new[] { new Modifier("+", mod) });
                }
            }

            diceAmount = 1;
            mod = minMax.Min - 1;
            diceType = minMax.Max - mod;
            return new DiceRoll(new[] { new Die(diceAmount, diceType) }, new[] { new Modifier("+", mod) });
        }
    }
}
