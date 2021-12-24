using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceCalculator
{
    public static class Helpers
    {
        public static int[] GetDiceToKeep(DiceRoll diceRoll, int[] dieRolls)
        {
            return RemoveDice(diceRoll.KeepHigh, diceRoll.NumDiceToKeep, ref dieRolls);
        }

        public static int[] GetDiceToDrop(DiceRoll diceRoll, int[] dieRolls)
        {
            var numDiceToDrop = diceRoll.NumDice - diceRoll.NumDiceToKeep;
            return RemoveDice(!diceRoll.KeepHigh, numDiceToDrop, ref dieRolls);
        }

        private static int[] RemoveDice(bool keep, int num, ref int[] dieRolls)
        {
            Array.Sort(dieRolls);
            dieRolls = keep
                ? dieRolls.TakeLast(num).ToArray()
                : dieRolls.Take(num).ToArray();
            return dieRolls;
        }

        public static int AddModifiers(float num, IEnumerable<Modifier> modifiers)
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
    }
}
