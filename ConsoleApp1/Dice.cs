

using System;

public class Dice
{
    public int[] Sides { get; private set; }

    public Dice(int[] sides)
    {
        if (sides.Length != 6) throw new ArgumentException("A dice must have exactly 6 sides.");
        Sides = sides;
    }

    public int Roll(Random random)
    {
        return Sides[random.Next(Sides.Length)];
    }

    public override string ToString()
    {
        return string.Join(",", Sides);
    }
}
