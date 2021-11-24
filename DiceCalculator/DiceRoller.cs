using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceCalculator
{
    public static class DiceRoller
    {
        public static int RollDice(DiceRoll diceRoll, int? seed = null)
        {
            var rolls = GetDiceRolls(diceRoll.Dice, seed);
            var rollBeforeMods = AddRolls(rolls);
            var roll = Helpers.AddModifiers(rollBeforeMods, diceRoll.Modifiers);
            return roll;
        }

        private static IEnumerable<int> GetDiceRolls(IEnumerable<Die> dice, int? seed = null)
        {
            var randomizer = seed == null ? new Random() : new Random(seed.Value);
            var rolls = new List<int>();
            foreach (var die in dice)
            {
                var dieRolls = new int[die.NumDice];
                for (int i = 0; i < die.NumDice; i++)
                {
                    var num = randomizer.Next(1, die.NumDieFaces + 1);
                    dieRolls[i] = num;
                }

                dieRolls = Helpers.DropNonKeepDice(die, dieRolls);

                if (die.Operation == Operation.Subtract)
                {
                    dieRolls = dieRolls.Select(r => r * -1).ToArray();
                }
                rolls.AddRange(dieRolls);
            }

            return rolls;
        }

        private static int AddRolls(IEnumerable<int> rolls)
        {
            int roll = 0;
            foreach (var r in rolls)
            {
                roll += r;
            }
            return roll;
        }
    }
}
