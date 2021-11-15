using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DiceCalculator
{
    public static class Calculator
    {
        public static MinMax CalculateDiceRoll(DiceRoll diceRoll)
        {
            if (diceRoll.Dice == null || diceRoll.Modifiers == null)
            {
                return new MinMax(0, 0, 0);
            }

            // check if any dice rolls are too large to be calculated in a reasonable amount of time
            foreach (var die in diceRoll.Dice)
            {
                if (Math.Pow(die.DiceType, die.TotalDiceAmount) > 70000000) // used to be int.MAX
                {
                    return new MinMax(0, 0, 0);
                }
            }

            var calcs = CalculateMinAndMaxValues(diceRoll);
            var min = calcs.First().Key;
            var max = calcs.Last().Key;

            float tempAvg = 0f;
            foreach (var item in calcs)
            {
                tempAvg += item.Key * item.Value.Percentage;
            }
            var avg = (float)Math.Round(tempAvg / 100, 3);

            // round percentages to three decimals after finding average
            foreach (var item in calcs)
            {
                item.Value.Percentage = (float)Math.Round(item.Value.Percentage, 3);
            }

            return new MinMax(min, avg, max, calcs);
        }

        private static IDictionary<int, Calculations> CalculateMinAndMaxValues(DiceRoll diceRoll)
        {
            var fullTotalsAndCount = new Dictionary<int, int>();
            foreach (var die in diceRoll.Dice)
            {
                var partialMatrix4 = new List<int[]>();
                var diceIndex = 0;
                CalculateRecursively(die, partialMatrix4, ref diceIndex, colList: new int[die.TotalDiceAmount]);

                var dieTotalsAndCount = AddPartialMatrixRows(partialMatrix4, die);

                AddPartialMatrixToFullMatrix(ref fullTotalsAndCount, dieTotalsAndCount);
            }

            return DoMinMaxCalculations(fullTotalsAndCount, diceRoll);
        }

        private static void CalculateRecursively(Die die, List<int[]> matrix, ref int diceIndex, int[] colList)
        {
            if (diceIndex >= die.TotalDiceAmount)
            {
                var remainingSortedDice = DropNonKeepDice(die, colList.ToArray());
                matrix.Add(remainingSortedDice);
                diceIndex--;
                colList[diceIndex] = 0;
                return;
            }

            for (int i = 1; i <= die.DiceType; i++)
            {
                colList[diceIndex] = i;
                diceIndex++;
                CalculateRecursively(die, matrix, ref diceIndex, colList);
            }
            if (diceIndex > 0)
            {
                diceIndex--;
                colList[diceIndex] = 0;
            }
        }

        private static int[] DropNonKeepDice(Die die, int[] dieRolls)
        {
            // TODO: merge with DiceRoller.GetDiceRolls
            if (die.KeepAmount != 0 && die.KeepAmount < die.TotalDiceAmount)
            {
                Array.Sort(dieRolls);
                dieRolls = die.KeepHigh
                    ? dieRolls.TakeLast(die.KeepAmount).ToArray()
                    : dieRolls.Take(die.KeepAmount).ToArray();
            }
            return dieRolls;
        }

        private static Dictionary<int, int> AddPartialMatrixRows(List<int[]> partialMatrix, Die die)
        {
            // add together each row of the partial matrix,
            // turning it negative if the die operation is subtract
            var dieTotalsAndCount = new Dictionary<int, int>();
            foreach (var list in partialMatrix)
            {
                var sum = 0;
                foreach (var num in list)
                {
                    sum += num;
                }
                if (die.Operation == Operation.Subtract)
                {
                    sum *= -1;
                }
                if (dieTotalsAndCount.ContainsKey(sum))
                {
                    dieTotalsAndCount[sum]++;
                }
                else
                {
                    dieTotalsAndCount.Add(sum, 1);
                }
            }
            return dieTotalsAndCount;
        }

        private static void AddPartialMatrixToFullMatrix(ref Dictionary<int, int> fullTotalsAndCount,
            Dictionary<int, int> dieTotalsAndCount)
        {
            if (fullTotalsAndCount.Count == 0)
            {
                fullTotalsAndCount = dieTotalsAndCount;
            }
            else
            {
                var temp = new Dictionary<int, int>();
                foreach (var kvp in fullTotalsAndCount)
                {
                    foreach (var kvp2 in dieTotalsAndCount)
                    {
                        var newKey = kvp.Key + kvp2.Key;
                        var addedValue = kvp.Value * kvp2.Value;
                        if (temp.ContainsKey(newKey))
                        {
                            temp[newKey] += addedValue;
                        }
                        else
                        {
                            temp.Add(newKey, addedValue);
                        }
                    }
                }
                fullTotalsAndCount = temp;
            }
        }

        private static IDictionary<int, Calculations> DoMinMaxCalculations(IDictionary<int, int> summedMatrix,
            DiceRoll diceRoll)
        {
            var calcs = new Dictionary<int, Calculations>();

            // calculate frequency of each dice num
            foreach (var kvp in summedMatrix)
            {
                var key = (int)AddModifiers(kvp.Key, diceRoll.Modifiers);
                if (calcs.ContainsKey(key))
                {
                    calcs[key].Frequency += kvp.Value;
                }
                else
                {
                    calcs.Add(key, new Calculations(kvp.Value, 0));
                }
            }

            // put dice nums in order from lowest to highest
            calcs = calcs.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // calculate percentages
            var totalNums = calcs.Values.Sum(v => v.Frequency);
            foreach (var calc in calcs)
            {
                var pct = (calc.Value.Frequency * 100f) / totalNums;
                calc.Value.Percentage = pct;
            }

            return calcs;
        }

        private static float AddModifiers(float num, IEnumerable<Modifier> modifiers)
        {
            foreach (var mod in modifiers)
            {
                switch (mod.Operation)
                {
                    case Operation.Add:
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
                return new DiceRoll(null, null);
            }

            if (minMax.Min == 1)
            {
                return new DiceRoll(new[] { new Die(1, minMax.Max, Operation.Add) }, new[] { new Modifier("+", 0) });
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
                    return new DiceRoll(new[] { new Die(diceAmount, diceType, Operation.Add) }, new[] { new Modifier("+", mod) });
                }
            }

            diceAmount = 1;
            mod = minMax.Min - 1;
            diceType = minMax.Max - mod;
            return new DiceRoll(new[] { new Die(diceAmount, diceType, Operation.Add) }, new[] { new Modifier("+", mod) });
        }
    }
}
