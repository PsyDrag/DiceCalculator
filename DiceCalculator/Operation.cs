namespace DiceCalculator
{
    public class Operation
    {
        private Operation(string value) { Value = value; }

        public string Value { get; internal set; }

        public static Operation Add      { get { return new Operation("+"); } }
        public static Operation Subtract { get { return new Operation("-"); } }
        public static Operation Multiply { get { return new Operation("*"); } }
        public static Operation Divide   { get { return new Operation("/"); } }
    }
}
