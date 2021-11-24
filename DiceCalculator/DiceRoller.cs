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
            var roll = AddModifiers(rollBeforeMods, diceRoll.Modifiers);
            return roll;
        }

        private static IEnumerable<int> GetDiceRolls(IEnumerable<Die> dice, int? seed = null)
        {
            var randomizer = seed == null ? new Random() : new Random(seed.Value);
            var rolls = new List<int>();
            foreach (var die in dice)
            {
                var dieRolls = new List<int>();
                for (int i = 0; i < die.NumDice; i++)
                {
                    var num = randomizer.Next(1, die.NumDieFaces + 1);
                    dieRolls.Add(num);
                }

                if (Helpers.NeedToDropDice(die))
                {
                    dieRolls.Sort();
                    var amtToRemove = die.NumDice - die.NumDiceToKeep;
                    if (die.KeepHigh)
                    {
                        dieRolls.RemoveRange(0, amtToRemove);
                    }
                    else
                    {
                        dieRolls.RemoveRange(die.NumDiceToKeep, amtToRemove);
                    }
                }

                if (die.Operation == Operation.Subtract)
                {
                    dieRolls = dieRolls.Select(r => r * -1).ToList();
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

        private static int AddModifiers(int roll, IEnumerable<Modifier> modifiers)
        {
            // TODO: merge with Calculator.AddModifiers
            foreach (var mod in modifiers)
            {
                switch (mod.Operation)
                {
                    case Operation.Add:
                        roll += mod.Number;
                        break;
                    case "-":
                        roll -= mod.Number;
                        break;
                    case "*":
                        roll *= mod.Number;
                        break;
                    case "/":
                        roll /= mod.Number;
                        break;
                    default:
                        break;
                }
            }
            return roll;
        }
    }
}
