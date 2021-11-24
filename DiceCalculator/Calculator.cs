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
                // used to be int.MaxValue (~2.14mil)
                if (Helpers.NeedToDropDice(die) && Math.Pow(die.NumDieFaces, die.NumDice) > 70000000)
                {
                    return new MinMax(0, 0, 0);
                }
            }

            IList<Calculations> calcs;
            try
            {
                calcs = CalculateMinAndMaxValues(diceRoll);
            }
            catch (OverflowException)
            {
                return new MinMax(0, 0, 0);
            }
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
            var diceRollTotalsAndCount = new Dictionary<int, long>();
            foreach (var die in diceRoll.Dice)
            {
                if (Helpers.NeedToDropDice(die))
                {
                    var permutations = new List<int[]>();
                    int diceIndex = 0;
                    CalculateRecursively(die, permutations, ref diceIndex, colList: new int[die.NumDice]);

                    var dieTotalsAndCount = AddUpPermutations(permutations, die);
                    AddDieTotalsToDiceRollTotals(ref diceRollTotalsAndCount, dieTotalsAndCount); 
                }
                else
                {
                    var dieTotalsAndCount = GetDieTotalsAndCount(die);
                    AddDieTotalsToDiceRollTotals(ref diceRollTotalsAndCount, dieTotalsAndCount);
                }
            }

            var initalCalcs = AddModifiers(diceRollTotalsAndCount, diceRoll);
            return CalculatePercentages(initalCalcs);
        }

        private static void CalculateRecursively(Die die, IList<int[]> matrix, ref int diceIndex, int[] colList)
        {
            if (diceIndex >= die.NumDice)
            {
                var remainingSortedDice = Helpers.DropNonKeepDice(die, colList.ToArray());
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

        private static Dictionary<int, long> AddUpPermutations(IList<int[]> permutations, Die die)
        {
            // add all nums of each permutation,
            // turning it negative if the die operation is subtract
            var dieTotalsAndCount = new Dictionary<int, long>();
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

        private static void AddDieTotalsToDiceRollTotals(ref Dictionary<int, long> diceRollTotalsAndCount,
            Dictionary<int, long> dieTotalsAndCount)
        {
            if (diceRollTotalsAndCount.Count == 0)
            {
                diceRollTotalsAndCount = dieTotalsAndCount;
            }
            else
            {
                var tempTotalsAndCount = new Dictionary<int, long>();
                foreach (var currentDieTotal in diceRollTotalsAndCount)
                {
                    foreach (var dieTotal in dieTotalsAndCount)
                    {
                        int newTotal = currentDieTotal.Key + dieTotal.Key;
                        long newCount = currentDieTotal.Value * dieTotal.Value;
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

        private static Dictionary<int, long> GetDieTotalsAndCount(Die die)
        {
            var totalsAndCount = new Dictionary<int, long>();

            // formula taken from https://www.omnicalculator.com/statistics/dice#how-to-calculate-dice-roll-probability
            // it's essentially a multiset - https://qr.ae/pGmpoz
            int n = die.NumDice;
            int s = die.NumDieFaces;
            int min = n;
            int max = n * s;
            for (int r = min; r <= max; r++)
            {
                int rTotal = r;
                long rCount = 0;
                // this is the same as flooring a double
                int summationLimit = (r - n) / s;
                for (int k = 0; k <= summationLimit; k++)
                {
                    int part1 = (int)Math.Pow(-1, k);
                    long part2 = BinomialCoefficient(n, k);
                    long part3 = BinomialCoefficient(r - (s * k) - 1, n - 1);
                    long part = part1 * part2 * part3;
                    rCount += part;
                }
                if (rCount < 0)
                {
                    throw new OverflowException();
                }
                if (die.Operation == Operation.Subtract)
                {
                    rTotal *= -1;
                }
                totalsAndCount.Add(rTotal, rCount);
            }

            return totalsAndCount;
        }

        // from https://math.stackexchange.com/a/927064
        private static long BinomialCoefficient(int n, int k)
        {
            if (k > n)
                return 0;
            if (k == 0 || k == n)
                return 1;
            if (k > n / 2)
                return BinomialCoefficient(n, n - k);
            return n * BinomialCoefficient(n - 1, k - 1) / k;
        }

        private static IList<Calculations> AddModifiers(IDictionary<int, long> summedMatrix, DiceRoll diceRoll)
        {
            var calcs = new List<Calculations>();
            foreach (var kvp in summedMatrix)
            {
                int key = Helpers.AddModifiers(kvp.Key, diceRoll.Modifiers);
                calcs.Add(new Calculations(key, kvp.Value, 0));
            }

            calcs = calcs.OrderBy(calc => calc.Value).ToList();
            return calcs;
        }

        private static IList<Calculations> CalculatePercentages(IList<Calculations> calcs)
        {
            //totalNums should just be NumDieFaces ^ NumDice
            //or if NumDiceToKeep, NumDieFaces ^ NumDiceToKeep
            long totalNums = calcs.Sum(v => v.Frequency);
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
