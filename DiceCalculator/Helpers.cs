using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceCalculator
{
    public static class Helpers
    {
        // TODO: should probably have this be an extension method.
        // or in ctor should set NumDiceToKeep to NumDice where needed.
        public static bool NeedToDropDice(DiceRoll diceRoll)
        {
            return diceRoll.NumDiceToKeep != 0 && diceRoll.NumDiceToKeep < diceRoll.NumDice;
        }

        public static int[] DropNonKeepDice(DiceRoll diceRoll, int[] dieRolls)
        {
            if (NeedToDropDice(diceRoll))
            {
                Array.Sort(dieRolls);
                dieRolls = diceRoll.KeepHigh
                    ? dieRolls.TakeLast(diceRoll.NumDiceToKeep).ToArray()
                    : dieRolls.Take(diceRoll.NumDiceToKeep).ToArray();
            }
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
