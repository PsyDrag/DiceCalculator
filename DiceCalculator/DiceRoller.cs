using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceCalculator
{
    public static class DiceRoller
    {
        public static Tuple<DieRollThingy[], int> RollDice(DiceRoll diceRoll, int? seed = null)
        {
            var rolls = GetDiceRolls(diceRoll.Dice, seed);
            var rollsAndMods = AddModsToRolls(rolls.Item1, diceRoll.Modifiers).ToArray();
            var sum = Helpers.AddModifiers(rolls.Item2, diceRoll.Modifiers);
            return Tuple.Create(rollsAndMods, sum);
        }

        private static Tuple<IList<DieRollThingy>, int> GetDiceRolls(IEnumerable<Die> dice, int? seed = null)
        {
            var randomizer = seed == null ? new Random() : new Random(seed.Value);
            IList<DieRollThingy> rolls = new List<DieRollThingy>();
            int sum = 0;
            foreach (var die in dice)
            {
                var dieRolls = new Tuple<int, DieRollOutput>[die.NumDice];
                for (int i = 0; i < die.NumDice; i++)
                {
                    var num = randomizer.Next(1, die.NumDieFaces + 1);
                    
                    var dieOutput = DieRollOutput.Normal;
                    if (num == 1)
                    {
                        dieOutput = DieRollOutput.CritFailure;
                    }
                    else if (num == die.NumDieFaces)
                    {
                        dieOutput = DieRollOutput.CritSuccess;
                    }

                    dieRolls[i] = Tuple.Create(num, dieOutput);
                }

                MarkNonKeepDiceAsDropped(die, dieRolls);
                var tempSum = dieRolls
                    .Where(roll => (roll.Item2 & DieRollOutput.Dropped) != DieRollOutput.Dropped)
                    .Select(roll => roll.Item1)
                    .Sum();
                rolls.Add(new DieRollThingy(die.Operation, dieRolls, tempSum));

                sum = die.Operation == Operation.Add
                    ? sum + tempSum
                    : sum - tempSum;
            }

            return Tuple.Create(rolls, sum);
        }

        private static void MarkNonKeepDiceAsDropped(Die die, Tuple<int, DieRollOutput>[] dieRolls)
        {
            if (Helpers.NeedToDropDice(die))
            {
                var rolls = dieRolls.Select(roll => roll.Item1).ToArray();
                Array.Sort(rolls);
                var numDiceToDrop = die.NumDice - die.NumDiceToKeep;
                var tempRolls = die.KeepHigh
                    ? rolls.Take(numDiceToDrop).ToList()
                    : rolls.TakeLast(numDiceToDrop).ToList();

                // compare rolls to dieRolls.Item1 and | .Item2 with DieRollOutput.Dropped
                for (var i = 0; i < dieRolls.Length; i++)
                {
                    if (tempRolls.Contains(dieRolls[i].Item1))
                    {
                        var newOutput = dieRolls[i].Item2 | DieRollOutput.Dropped;
                        dieRolls[i] = Tuple.Create(dieRolls[i].Item1, newOutput);
                        tempRolls.Remove(dieRolls[i].Item1);
                    }
                }
            }
        }

        private static IList<DieRollThingy> AddModsToRolls(IList<DieRollThingy> rolls, IEnumerable<Modifier> modifiers)
        {
            foreach (var mod in modifiers)
            {
                var modRoll = new[] { Tuple.Create(mod.Number, DieRollOutput.Normal) };
                rolls.Add(new DieRollThingy(mod.Operation, modRoll, mod.Number));
            }
            return rolls;
        }
    }

    public class DieRollThingy
    {
        public DieRollThingy(string op, Tuple<int, DieRollOutput>[] rolls, int dieRollSum)
            => (Operation, DieRolls, DieRollSum) = (op, rolls, dieRollSum);

        public string Operation { get; set; }
        public Tuple<int, DieRollOutput>[] DieRolls { get; set; }
        public int DieRollSum { get; set; }
    }

    [Flags]
    public enum DieRollOutput
    {
        Normal      = 1,
        CritSuccess = 2,
        CritFailure = 4,
        Dropped     = 8
    }
}
