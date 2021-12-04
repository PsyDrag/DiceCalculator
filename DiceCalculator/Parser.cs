using System.Collections.Generic;

namespace DiceCalculator
{
    public static class Parser
    {
        public static DiceRoll ParseDiceRoll(string line)
        {
            line = line.ToLower();

            if (line.Contains("ac") || line.Contains("dc"))
            {
                return new DiceRoll(null, null);
            }
            else
            {
                line = line.Replace(" ", "");
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == '+' || line[i] == '-' || line[i] == '*' || line[i] == '/')
                    {
                        line = line.Insert(i + 1, " ");
                        line = line.Insert(i, " ");
                        i += 2;
                    }
                }

                var dice = new List<Die>();
                var modifiers = new List<Modifier>();
                bool shouldLookBack = false;
                var components = line.Split(' ');
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].Contains('d'))
                    {
                        var die = components[i].Split('d');
                        int amt = die[0] == string.Empty ? 1 : int.Parse(die[0]);
                        int type = 0;
                        int keep = 0;
                        bool keepHigh = false;
                        string diceOp;

                        if (die[1].Contains('k'))
                        {
                            var keepNums = die[1].Split('k');
                            type = int.Parse(keepNums[0]);

                            int keepAmt = int.Parse(keepNums[1][1..]);
                            keep = keepAmt > amt ? amt : keepAmt;
                            keepHigh = keepNums[1][0] == 'h';
                        }
                        else
                        {
                            type = int.Parse(die[1]);
                        }

                        if (shouldLookBack)
                        {
                            string lookBackOp = components[i - 1];
                            diceOp = lookBackOp switch
                            {
                                Operation.Add      => Operation.Add,
                                Operation.Subtract => Operation.Subtract,
                                Operation.Multiply => Operation.Multiply,
                                Operation.Divide   => Operation.Divide,
                                _                  => Operation.Add
                            };
                        }
                        else
                        {
                            diceOp = Operation.Add;
                        }
                        dice.Add(new Die(amt, type, keepHigh, keep, diceOp));
                        shouldLookBack = false;
                    }
                    else if ((components[i] == "+" || components[i] == "-") && components[i+1].Contains('d'))
                    {
                        shouldLookBack = true;
                    }
                    else if (components[i] == "+" || components[i] == "-" || components[i] == "*" || components[i] == "/")
                    {
                        int.TryParse(components[i + 1], out int num);
                        if (num != 0)
                        {
                            modifiers.Add(new Modifier(components[i], num));
                            i++; 
                        }
                    }
                    else if (int.TryParse(components[i], out int num))
                    {
                        modifiers.Add(new Modifier(Operation.Add, num));
                    }
                }

                return new DiceRoll(dice, modifiers);
            }
        }

        public static MinMax ParseMinMax(string line)
        {
            line = line.ToLower();
            line = line.Replace(" ", "");
            for (int i = 0; i < line.Length; i++)
            {
                if (i != 0 && char.IsLetter(line[i]) && char.IsNumber(line[i-1]))
                {
                    line = line.Insert(i, " ");
                    i++;
                }
            }

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
