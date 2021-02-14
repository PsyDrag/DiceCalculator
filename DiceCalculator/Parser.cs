using System.Collections.Generic;

namespace DiceCalculator
{
    public static class Parser
    {
        public static DiceRoll ParseDiceRoll(string line)
        {
            line = line.ToLower();

            if (line.Contains("kl") || line.Contains("ac") || line.Contains("dc"))
            {
                Printer.PrintError("Something in your input is not yet implemented");
                return new DiceRoll(null, null);
            }
            else
            {
                var dice = new List<Die>();
                var modifiers = new List<Modifier>();
                var components = line.Split(' ');
                for (var i = 0; i < components.Length; i++)
                {
                    if (components[i].Contains('d'))
                    {
                        var die = components[i].Split('d');
                        int amt, keep = 0;
                        if (die[0].Contains("kh"))
                        {
                            var asdf = die[0].Split("kh");
                            amt = int.Parse(asdf[0]);
                            keep = int.Parse(asdf[1]);
                        }
                        else
                        {
                            amt = die[0] == string.Empty ? 1 : int.Parse(die[0]);
                        }
                        var type = int.Parse(die[1]);
                        dice.Add(new Die(amt, keep, type));
                    }
                    else if (components[i] == "+" && components[i+1].Contains('d'))
                    {
                        // this assumes all dice rolls are added together
                    }
                    else if (components[i] == "+" || components[i] == "-" || components[i] == "*" || components[i] == "/")
                    {
                        var num = int.Parse(components[i + 1]);
                        modifiers.Add(new Modifier(components[i], num));
                        i++;
                    }
                }

                return new DiceRoll(dice, modifiers);
            }
        }

        public static MinMax ParseMinMax(string line)
        {
            line = line.ToLower();

            int min = 0;
            float avg = 0f;
            int max = 0;
            var parts = line.Split(' ');
            foreach (var part in parts)
            {
                var asdf = part.Split(':');
                if (asdf[0] == "min")
                {
                    min = int.Parse(asdf[1]);
                }
                if (asdf[0] == "avg")
                {
                    avg = float.Parse(asdf[1]);
                }
                if (asdf[0] == "max")
                {
                    max = int.Parse(asdf[1]);
                }
            }

            return new MinMax(min, avg, max);
        }
    }
}
