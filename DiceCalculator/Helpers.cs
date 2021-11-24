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
    }
}
