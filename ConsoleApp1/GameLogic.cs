using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

public class GameLogic(List<Dice> diceList)
{
    private readonly List<Dice> _diceList = diceList ?? throw new ArgumentNullException(nameof(diceList), "Dice list cannot be null.");
    private readonly FairRandomGenerator _randomGenerator = new();
    private readonly Random _random = new();

    Dice? computerDice;

    Dice?  userDice;


    private Boolean _playerTurn = false;

    void DisplayMenuDice(){
            // Display available dice
            Console.WriteLine("\nAvailable dice:");
            for (int i = 0; i < _diceList.Count; i++)
                Console.WriteLine($"[{i + 1}] - {_diceList[i]}");
            
            Console.WriteLine("X - Exit");
            Console.WriteLine("H - Help");
            Console.Write("Your selection: ");
    }

    static void DisplayIntroduction(){

        Console.WriteLine("Welcome to the Dice Game!");
        Console.WriteLine("Let's determine who makes the first move.");
        Console.WriteLine("I selected a random value in the range 0..1 .");
    }

    static void DisplayInitialMenu(){

        Console.WriteLine("Try to guess my selection.");
        Console.WriteLine("0 - 0");
        Console.WriteLine("1 - 1");
        Console.WriteLine("X - Exit");
        Console.WriteLine("H - Help");
        Console.Write("Your selection: ");
    }

    

    void ProcessInitialTossInput(int computerNumber)
    {
        string? input;
        while (true) // Loop until valid input is provided
        {
            Console.Write("Your selection: ");
            input = Console.ReadLine()?.Trim();

            if (input == "X")
            {
                Console.WriteLine("Thanks for playing!");
                Environment.Exit(0);
            }
            else if (input == "H")
            {
                // Display probabilities
                var probabilities = ProbabilityCalculator.CalculateProbabilities(_diceList);
                AsciiTableGenerator.DisplayProbabilities(probabilities, _diceList);
                Console.WriteLine("");
            }
            else if (input == "0" || input == "1")
            {
                if (input == computerNumber.ToString())
                {
                    Console.WriteLine("You won the toss! You start.");
                    _playerTurn = true;
                }
                else
                {
                    Console.WriteLine("You lost the toss! I start.");
                }
                break; // Valid input, exit the loop
            }
            else
            {
                // Invalid input
                Console.WriteLine("Invalid input. Please enter '0', '1', 'X' to exit, or 'H' for help.");
            }
        }
    }

    void ChooseDice(){
         if(_playerTurn){
            DisplayMenuDice();
         }
         
    }


    void ProcessInputSelection(){
        
    }

   


    void InicializeGame()
    {
        DisplayIntroduction();

        // Generate HMAC for computer's number
        var (computerNumber, hmac, key) = _randomGenerator.Generate(0, 2);

        Console.WriteLine($"HMAC: {hmac}");

        DisplayInitialMenu();

        ProcessInitialTossInput(computerNumber);

        // Calculate result

        Console.WriteLine($"Computer number: {computerNumber}, Key: {BitConverter.ToString(key).Replace('-', '\0')}");
        
    }

    public void StartGame()
    {
        InicializeGame();
    

        while (true)
        {
  
            ChooseDice();
            
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid input. Please try again.");
                continue;
            }

            if (input == "H")
            {
                // Display probabilities
                var probabilities = ProbabilityCalculator.CalculateProbabilities(_diceList);
                AsciiTableGenerator.DisplayProbabilities(probabilities, _diceList);
                continue;
            }

            if (input == "X")
            {
                Console.WriteLine("Thanks for playing!");
                break;
            }    

            // Parse user's dice choice
            if (int.TryParse(input, out int userDiceIndex) && userDiceIndex > 0 && userDiceIndex <= _diceList.Count)
            {
                var computerDiceIndex = _random.Next(_diceList.Count);
                this.userDice = _diceList[userDiceIndex - 1];
                this.computerDice = _diceList[computerDiceIndex];

                Console.WriteLine($"You selected: {userDice}");
                Console.WriteLine($"Computer selected: {computerDice}");

                // Generate HMAC for computer's number
                var (computerNumber, hmac, key) = _randomGenerator.Generate(1, 7);

                Console.WriteLine($"HMAC: {hmac}");
                Console.Write("Enter your number (1-6): ");
                string? userNumberInput = Console.ReadLine()?.Trim();

                if (!int.TryParse(userNumberInput, out int userNumber) || userNumber < 1 || userNumber > 6)
                {
                    Console.WriteLine("Invalid number. Please enter a number between 1 and 6.");
                    continue;
                }

                // Calculate result
                int result = (userNumber + computerNumber) % 6;

                Console.WriteLine($"Result: {result}");
                Console.WriteLine($"Computer number: {computerNumber}, Key: {BitConverter.ToString(key).Replace('-', '\0')}");

                if (result > userNumber)
                    Console.WriteLine("Computer wins!");
                else
                    Console.WriteLine("You win!");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid dice number or command.");
            }
        }
    }
}
