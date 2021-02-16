namespace DiceCalculator
{
    public class Modifier
    {
        public Modifier(string operation, int number)
            => (Operation, Number) = (operation, number);

        public string Operation { get; set; }
        public int Number { get; set; }
    }
}
