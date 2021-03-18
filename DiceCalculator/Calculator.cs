using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceCalculator
{
    public static class Calculator
    {
        public static MinMax CalculateDiceRoll(DiceRoll diceRoll)
        {
            // check if dice roll is too large
            foreach (var die in diceRoll.Dice)
            {
                if (Math.Pow(die.DiceType, die.TotalDiceAmount) > int.MaxValue) // basically >2.5billion
                {
                    Printer.PrintError("Dice roll too large to calculate in a decent amount of time");
                    return new MinMax(0, 0, 0);
                }
            }

            int min = 0;
            int max = 0;
            Dictionary<int, Calculations> calcs = null;
            calcs = CalculateMinAndMaxValues(diceRoll);
            min = calcs.First().Key;
            max = calcs.Last().Key;
            

            float avg;
            var isKeepRoll = diceRoll.Dice.Any(d => d.KeepAmount != 0);
            if (!isKeepRoll)
            {
                // mid-range
                avg = ((float)max + min) / 2;
            }
            else
            {
                //use calcs instead of calculating again
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
                    tempAvg = die.Operation == Operation.Subtract
                        ? tempAvg - dieAvg
                        : tempAvg + dieAvg;
                }
                avg = AddModifiers(tempAvg, diceRoll.Modifiers);
            }

            return new MinMax(min, avg, max, calcs);
        }
 
        private static Dictionary<int, Calculations> CalculateMinAndMaxValues(DiceRoll diceRoll)
        {
            var summedMatrix = new List<int>();
            foreach (var die in diceRoll.Dice)
            {
                var dice = new List<object[]>();
                for (int i = 0; i < die.TotalDiceAmount; i++)
                {
                    dice.Add(new object[] { die.Operation, die.DiceType });
                }

                int rows = (int)Math.Pow(die.DiceType, die.TotalDiceAmount);
                var partialMatrix = new List<int>[rows];
                var diceIndex = 0;
                var rowIndex = 0;
                CalculateRecursively(dice, ref diceIndex, partialMatrix, ref rowIndex, colList: new List<int>());

                // TODO: merge with DiceRoller.GetDiceRolls
                if (die.KeepAmount != 0 && die.KeepAmount < die.TotalDiceAmount)
                {
                    foreach (var row in partialMatrix)
                    {
                        row.Sort();
                        var amtToRemove = die.TotalDiceAmount - die.KeepAmount;
                        if (die.KeepHigh)
                        {
                            row.RemoveRange(0, amtToRemove);
                        }
                        else
                        {
                            row.RemoveRange(die.KeepAmount, amtToRemove);
                        }
                    }
                }

                // add together each row of the partial matrix,
                // turning it negative if the die operation is subtract
                var dieSumMatrix = partialMatrix.Select(row =>
                {
                    var sum = 0;
                    foreach (var num in row)
                    {
                        sum += num;
                    }
                    return sum;
                });

                if (summedMatrix.Count == 0)
                {
                    summedMatrix.AddRange(dieSumMatrix);
                }
                else
                {
                    var tempSummedMatrix = new List<int>();
                    foreach (var item in summedMatrix)
                    {
                        foreach (var item2 in dieSumMatrix)
                        {
                            tempSummedMatrix.Add(item + item2);
                        }
                    }
                    summedMatrix = tempSummedMatrix;
                }
            }

            var calcs = new Dictionary<int, Calculations>();

            // calculate frequency of each dice num
            foreach (var num in summedMatrix)
            {
                var key = (int)AddModifiers(num, diceRoll.Modifiers);
                if (calcs.ContainsKey(key))
                {
                    calcs[key].Frequency++;
                }
                else
                {
                    calcs.Add(key, new Calculations(1, 0));
                }
            }

            // put dice nums in order from lowest to highest
            calcs = calcs.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // calculate percentages
            var totalNums = calcs.Values.Sum(v => v.Frequency);
            foreach (var calc in calcs)
            {
                var pct = (calc.Value.Frequency * 100f) / totalNums;
                calc.Value.Percentage = (float)Math.Round(pct, 3);
            }

            return calcs;
        }

        private static void CalculateRecursively(List<object[]> dice, ref int diceIndex, List<int>[] matrix, ref int rowIndex, List<int> colList)
        {
            if (diceIndex >= dice.Count)
            {
                matrix[rowIndex] = colList.ToList();
                rowIndex++;
                colList.RemoveAt(colList.Count - 1);
                diceIndex--;
                return;
            }

            var die = dice[diceIndex];
            var isSubtract = die[0].ToString() == Operation.Subtract;
            for (int i = 1; i <= (int)die[1]; i++)
            {
                var num = isSubtract ? -1 * i : i;
                colList.Add(num);
                diceIndex++;
                CalculateRecursively(dice, ref diceIndex, matrix, ref rowIndex, colList);
            }
            if (colList.Count > 0)
            {
                colList.RemoveAt(colList.Count - 1);
                diceIndex--;
            }
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
