using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceCalculator
{
    public static class Helpers
    {
        // TODO: should probably have this be an extension method.
        // or in ctor should set NumDiceToKeep to NumDice where needed.
        public static bool NeedToDropDice(Die die)
        {
            return die.NumDiceToKeep != 0 && die.NumDiceToKeep < die.NumDice;
        }

        public static int[] DropNonKeepDice(Die die, int[] dieRolls)
        {
            if (NeedToDropDice(die))
            {
                Array.Sort(dieRolls);
                dieRolls = die.KeepHigh
                    ? dieRolls.TakeLast(die.NumDiceToKeep).ToArray()
                    : dieRolls.Take(die.NumDiceToKeep).ToArray();
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
