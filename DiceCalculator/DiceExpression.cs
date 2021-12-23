using System.Collections.Generic;

namespace DiceCalculator
{
    public class DiceExpression
    {
        public DiceExpression(IEnumerable<DiceRoll> diceRolls, IEnumerable<Modifier> modifiers)
            => (DiceRolls, Modifiers) = (diceRolls, modifiers);

        public IEnumerable<DiceRoll> DiceRolls { get; set; }
        public IEnumerable<Modifier> Modifiers { get; set; }
    }
}