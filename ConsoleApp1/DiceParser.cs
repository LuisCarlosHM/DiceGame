using System;
using System.Collections.Generic;
using System.Linq;

public class DiceParser
{
    public static List<Dice> Parse(string[] args)
    {
        var diceList = new List<Dice>();
        foreach (var arg in args)
        {
            var sides = arg.Split(',')
                           .Select(s => int.TryParse(s, out int n) ? n : (int?)null)
                           .ToList();

            if (sides.Count != 6 || sides.Any(side => side == null))
                throw new ArgumentException($"Invalid dice configuration: {arg}");

            diceList.Add(new Dice(sides.Cast<int>().ToArray()));
        }
        return diceList;
    }
}
