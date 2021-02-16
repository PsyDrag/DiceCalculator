using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceCalculator
{
    public static class Calculator
    {
        public static MinMax CalculateDiceRoll(DiceRoll diceRoll)
        {
            var isKeepRoll = diceRoll.Dice.Any(d => d.KeepAmount != 0);

            CalculateMinAndMaxValues(diceRoll, out var min, out var max);

            float avg;
            if (!isKeepRoll)
            {
                // mid-range
                avg = ((float)max + min) / 2;
            }
            else
            {
                foreach (var die in diceRoll.Dice)
                {
                    if (Math.Pow(die.DiceType, die.TotalDiceAmount) > int.MaxValue) // basically > 2.5billion
                    {
                        Printer.PrintError("Dice roll too large to calculate in a decent amount of time");
                        return new MinMax(0, 0, 0);
                    }
                }

                float tempAvg = 0;
                foreach (var die in diceRoll.Dice)
                {
                    var diceType = die.DiceType;
                    var tdr = die.TotalDiceAmount;
                    var ntdr = (long)Math.Pow(diceType, tdr);
                
                    var permutations = new int[tdr];
                    var totalOfValues = 0L;
                    // input first permutation
                    for (int i = 0; i < tdr; i++)
                    {
                        permutations[i] = 1;
                    }
                    totalOfValues += die.KeepAmount;

                    // for the remaining permutations, increase the right-most number until is reaches n(the diceType)
                    // if the number is n, increase the number to its left and set this number to 1
                    // copy the array and sort
                    // remove numbers from left or right depending on if you are keeping high or low
                    // add remaining numbers to totalOfValues
                    for (int i = 2; i <= ntdr; i++)
                    {
                        // increase
                        IncreasePermutations(permutations, tdr - 1, diceType);

                        // copy and sort
                        var clone = (int[])permutations.Clone();
                        Array.Sort(clone);

                        // add
                        var first = die.KeepHigh ? tdr - die.KeepAmount : 0;
                        var last = die.KeepHigh ? tdr : die.KeepAmount;
                        for (int j = first; j < last; j++)
                        {
                            totalOfValues += clone[j];
                        }
                    }
                    var dieAvg = (float)Math.Round(totalOfValues / (float)ntdr, 3);
                    tempAvg = die.Operation == Operation.Subtract.Value
                        ? tempAvg - dieAvg
                        : tempAvg + dieAvg;
                }
                avg = AddModifiers(tempAvg, diceRoll.Modifiers);
            }

            return new MinMax(min, avg, max);
        }

        private static void CalculateMinAndMaxValues(DiceRoll diceRoll, out int min, out int max)
        {
            min = 0;
            max = 0;
            foreach (var die in diceRoll.Dice)
            {
                var minAmt = die.KeepAmount == 0 ? die.TotalDiceAmount : die.KeepAmount;
                var maxAmt = (die.KeepAmount == 0 ? die.TotalDiceAmount : die.KeepAmount) * die.DiceType;
                if (die.Operation == Operation.Subtract.Value)
                {
                    min -= maxAmt;
                    max -= minAmt;
                }
                else
                {
                    min += minAmt;
                    max += maxAmt;
                }
            }

            min = (int)AddModifiers(min, diceRoll.Modifiers);
            max = (int)AddModifiers(max, diceRoll.Modifiers);
        }

        private static float AddModifiers(float num, IEnumerable<Modifier> modifiers)
        {
            foreach (var mod in modifiers)
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

        private static void IncreasePermutations(int[] permutations, int i, int diceType)
        {
            if (permutations[i] != diceType)
            {
                permutations[i]++;
                return;
            }

            permutations[i] = 1;
            IncreasePermutations(permutations, i - 1, diceType);
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
                return new DiceRoll(new[] { new Die(1, minMax.Max, Operation.Add.Value) }, new[] { new Modifier("+", 0) });
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
                    return new DiceRoll(new[] { new Die(diceAmount, diceType, Operation.Add.Value) }, new[] { new Modifier("+", mod) });
                }
            }

            diceAmount = 1;
            mod = minMax.Min - 1;
            diceType = minMax.Max - mod;
            return new DiceRoll(new[] { new Die(diceAmount, diceType, Operation.Add.Value) }, new[] { new Modifier("+", mod) });
        }
    }
}
