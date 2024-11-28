using System;
using ConsoleTables;

public class AsciiTableGenerator
{
    public static void DisplayProbabilities(double[,] probabilities, List<Dice> diceList)
    {
        // Create headers dynamically
        var headers = new List<string> { "Dice" };
        Console.ForegroundColor = ConsoleColor.Yellow;
        
        headers.AddRange(diceList.Select((d, index) => d.ToString()));
        
        // Create table
        var table = new ConsoleTable(headers.ToArray());

        // Populate rows
        for (int i = 0; i < diceList.Count; i++)
        {
            var row = new List<object> { diceList[i] }; // First column (row header)
            for (int j = 0; j < diceList.Count; j++)
            {
                if (i == j)
                    row.Add("-"); // Diagonal element
                else
                    row.Add($"{probabilities[i, j]:0.00}");
            }
            table.AddRow(row.ToArray());
        }

        // Write the table
        table.Write(Format.Alternative);

        Console.ResetColor();
    
    }
}
