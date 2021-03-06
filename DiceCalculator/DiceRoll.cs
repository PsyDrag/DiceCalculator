﻿using System.Collections.Generic;

namespace DiceCalculator
{
    public class DiceRoll
    {
        public DiceRoll(IEnumerable<Die> dice, IEnumerable<Modifier> modifiers)
            => (Dice, Modifiers) = (dice, modifiers);

        public IEnumerable<Die> Dice { get; set; }
        public IEnumerable<Modifier> Modifiers { get; set; }
    }
}