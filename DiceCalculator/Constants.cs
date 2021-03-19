namespace DiceCalculator
{
    public static class Constants
    {
        public static readonly string DiceRollExamples =
            "Dice Roll Examples:\n" +
            "  1d4 + 7                standard roll plus modifier\n" +
            "  3d6                    no modifier\n" +
            "  d20                    straight up\n" +
            "  d6 * 2                 multiply\n" +
            "  d20 + 3d6 + 5          multiple dice rolls\n" +
            "  d6 - 3 * 2             multiple modifiers (calculations are made from left to right)\n" +
            "  4d6kh3                 standard ability roll\n" +
            "  2d20kl1                with disadvantage\n" +
            "  6d4kl5 + 3d6kh2 + 2    you can do this..but why?";
            //"  d20 + 6 ac 18          attack roll against AC 18\n" +
            //"  d20 + 4 dc 15          check against DC 15";

        public static readonly string MinMaxExamples =
            "Min-Max Examples:\n" +
            "  min:10 max:48          2d20 + 8 is a possible result\n" +
            "  min:6  max:17          1d12 + 5 is a possible result\n" +
            "  min:2  max:17          1d16 + 1 is a possible result";
        //  min:6 avg:11.5 max:17  d6 + 2d4 + 3 as a possible result
        //  min:6 avg:15 max:17    d12 + 5 (min and max are prioritized over avg)

        public static readonly string Help =
            "Usage: [options]\n" +
            "\n" +
            "Options:\n" +
            "  h|help                            Display help\n" +
            "  e|exit                            Exit program\n" +
            "  dr|diceroll [diceRollComponents]  You've been doing this for years so you ought to be able to\n" +
            "  mm|minmax [minMaxComponents]      Must always include min and max\n" +
            "  drex|drexamples                   Show examples of dice rolls\n" +
            "  mmex|mmexamples                   Show examples of min-max values\n" +
            "\n" +
            "Dice Roll Components:\n" +
            "  d                                 Do I really need to explain this one?\n" +
            "  kh                                Keep highest x number of dice rolls (e.g. 4d6kh3)\n" +
            "  kl                                Keep lowest x number of dice rolls (e.g. 2d20kl1)\n" +
            //"  ac                                Show pass/fail % against an AC\n" +
            //"  dc                                Show pass/fail % against a DC\n" +
            "\n" +
            "Min-Max Components:\n" +
            "  min                               Minimum value\n" +
            //"  avg                               Average value\n" +
            "  max                               Maximum value";
    }
}
