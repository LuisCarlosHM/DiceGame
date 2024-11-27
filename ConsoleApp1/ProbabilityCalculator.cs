using System;

public class ProbabilityCalculator
{
    public static double[,] CalculateProbabilities(List<Dice> diceList)
    {
        int n = diceList.Count;
        var probabilities = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j) continue;

                int wins = 0, total = 0;

                foreach (var side1 in diceList[i].Sides)
                {
                    foreach (var side2 in diceList[j].Sides)
                    {
                        total++;
                        if (side1 > side2) wins++;
                    }
                }
                probabilities[i, j] = (double)wins / total;
            }
        }
        return probabilities;
    }
}
