using System;
using System.Collections.Generic;
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
                if (Math.Pow(die.NumDieFaces, die.NumDice) > 70000000) // used to be int.MAX
                {
                    return new MinMax(0, 0, 0);
                }
            }

            var calcs = CalculateMinAndMaxValues(diceRoll);
            int min = calcs.First().Value;
            int max = calcs.Last().Value;

            float tempAvg = 0f;
            foreach (var calc in calcs)
            {
                tempAvg += calc.Value * calc.Percentage;
            }
            var avg = (float)Math.Round(tempAvg / 100, 3);

            // round percentages to three decimals after finding average
            foreach (var item in calcs)
            {
                item.Percentage = (float)Math.Round(item.Percentage, 3);
            }

            return new MinMax(min, avg, max, calcs);
        }

        private static IList<Calculations> CalculateMinAndMaxValues(DiceRoll diceRoll)
        {
            var diceRollTotalsAndCount = new Dictionary<int, int>();
            foreach (var die in diceRoll.Dice)
            {
                var permutations = new List<int[]>();
                int diceIndex = 0;
                CalculateRecursively(die, permutations, ref diceIndex, colList: new int[die.NumDice]);

                var dieTotalsAndCount = AddUpPermutations(permutations, die);
                AddDieTotalsToDiceRollTotals(ref diceRollTotalsAndCount, dieTotalsAndCount);
            }

            var initalCalcs = AddModifiers(diceRollTotalsAndCount, diceRoll);
            return CalculatePercentages(initalCalcs);
        }

        private static void CalculateRecursively(Die die, IList<int[]> matrix, ref int diceIndex, int[] colList)
        {
            if (diceIndex >= die.NumDice)
            {
                var remainingSortedDice = DropNonKeepDice(die, colList.ToArray());
                matrix.Add(remainingSortedDice);
                diceIndex--;
                colList[diceIndex] = 0;
                return;
            }

            for (int i = 1; i <= die.NumDieFaces; i++)
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
            if (die.NumDiceToKeep != 0 && die.NumDiceToKeep < die.NumDice)
            {
                Array.Sort(dieRolls);
                dieRolls = die.KeepHigh
                    ? dieRolls.TakeLast(die.NumDiceToKeep).ToArray()
                    : dieRolls.Take(die.NumDiceToKeep).ToArray();
            }
            return dieRolls;
        }

        private static Dictionary<int, int> AddUpPermutations(IList<int[]> permutations, Die die)
        {
            // add all nums of each permutation,
            // turning it negative if the die operation is subtract
            var dieTotalsAndCount = new Dictionary<int, int>();
            foreach (var list in permutations)
            {
                int sum = 0;
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

        private static void AddDieTotalsToDiceRollTotals(ref Dictionary<int, int> diceRollTotalsAndCount,
            Dictionary<int, int> dieTotalsAndCount)
        {
            if (diceRollTotalsAndCount.Count == 0)
            {
                diceRollTotalsAndCount = dieTotalsAndCount;
            }
            else
            {
                var tempTotalsAndCount = new Dictionary<int, int>();
                foreach (var currentDieTotal in diceRollTotalsAndCount)
                {
                    foreach (var dieTotal in dieTotalsAndCount)
                    {
                        int newTotal = currentDieTotal.Key + dieTotal.Key;
                        int newCount = currentDieTotal.Value * dieTotal.Value;
                        if (tempTotalsAndCount.ContainsKey(newTotal))
                        {
                            tempTotalsAndCount[newTotal] += newCount;
                        }
                        else
                        {
                            tempTotalsAndCount.Add(newTotal, newCount);
                        }
                    }
                }
                diceRollTotalsAndCount = tempTotalsAndCount;
            }
        }

        private static IList<Calculations> AddModifiers(IDictionary<int, int> summedMatrix, DiceRoll diceRoll)
        {
            var calcs = new List<Calculations>();
            foreach (var kvp in summedMatrix)
            {
                int key = AddModifiers(kvp.Key, diceRoll.Modifiers);
                calcs.Add(new Calculations(key, kvp.Value, 0));
            }

            calcs = calcs.OrderBy(calc => calc.Value).ToList();
            return calcs;
        }

        private static int AddModifiers(float num, IEnumerable<Modifier> modifiers)
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
            return (int)num;
        }

        private static IList<Calculations> CalculatePercentages(IList<Calculations> calcs)
        {
            int totalNums = calcs.Sum(v => v.Frequency);
            foreach (var calc in calcs)
            {
                float pct = (calc.Frequency * 100f) / totalNums;
                calc.Percentage = pct;
            }
            return calcs;
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

            int numberOfDice;
            int numberOfDieFaces;
            int mod;
            for (int i = 1; i <= minMax.Min; i++)
            {
                mod = minMax.Min - i;
                int newMax = minMax.Max - mod;
                if (newMax % i == 0 && diceTypes.Contains(newMax / i))
                {
                    numberOfDice = i;
                    numberOfDieFaces = newMax / i;
                    return new DiceRoll(new[] { new Die(numberOfDice, numberOfDieFaces, Operation.Add) }, new[] { new Modifier("+", mod) });
                }
            }

            numberOfDice = 1;
            mod = minMax.Min - 1;
            numberOfDieFaces = minMax.Max - mod;
            return new DiceRoll(new[] { new Die(numberOfDice, numberOfDieFaces, Operation.Add) }, new[] { new Modifier("+", mod) });
        }
    }
}
