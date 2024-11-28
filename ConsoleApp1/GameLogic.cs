using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.XPath;

public class GameLogic(List<Dice> diceList)
{
    private readonly List<Dice> _diceList = diceList ?? throw new ArgumentNullException(nameof(diceList), "Dice list cannot be null.");
    private readonly FairRandomGenerator _randomGenerator = new();
    private readonly Random _random = new();

    Dice? computerDice;

    Dice?  userDice;


    private Boolean _playerTurn = false;

    #region Menus
    void DisplayMenuSelectDice(){

            var availableDice = _diceList.Where(dice => dice != computerDice).ToList();

            // Display available dice
            Console.WriteLine("\nAvailable dice:");
            for (int i = 0; i < availableDice.Count; i++)
                Console.WriteLine($"[{i + 1}] - {availableDice[i]}");
            
            Console.WriteLine("X - Exit");
            Console.WriteLine("H - Help");
            Console.Write("Your selection: ");
    }

    static void DisplayIntroduction(){

        Console.WriteLine("\nWelcome to the Dice Game!");
        Console.WriteLine("Let's determine who makes the first move.");
        Console.WriteLine("I selected a random value in the range 0..1\n");
    }

    static void DisplayInitialMenu(){

        Console.WriteLine("\nTry to guess my selection.");
        Console.WriteLine("0 - 0");
        Console.WriteLine("1 - 1");
        Console.WriteLine("X - Exit");
        Console.WriteLine("H - Help\n");
        Console.Write("Your selection: ");
    }

    static void DisplayDiceMenu(){
            
            Console.WriteLine("\nChoose a dice:");
            Console.WriteLine("0 - 0");
            Console.WriteLine("1 - 1");
            Console.WriteLine("2 - 2");
            Console.WriteLine("3 - 3");
            Console.WriteLine("4 - 4");
            Console.WriteLine("5 - 5");
            Console.WriteLine("X - Exit");
            Console.WriteLine("H - Help\n");
            Console.Write("Your selection: ");
    }

    #endregion
    
    #region InputProcessing
    string ProcessInputDiceMenu(){

        while(true){
            DisplayDiceMenu();
            string? input = Console.ReadLine()?.Trim();
            Console.WriteLine("");

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
                Environment.Exit(0);
            }    

            if(int.TryParse(input, out int userDiceIndex) && userDiceIndex >= 0 && userDiceIndex < 6){
                return input;
            }

            Console.WriteLine("Invalid input. Please try again.");
        }

    }
    void ProcessInitialTossInput(int computerNumber)
    {
        string? input;
        while (true) // Loop until valid input is provided
        {
            DisplayInitialMenu();
            input = Console.ReadLine()?.Trim();
            Console.WriteLine("");

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

    
    void ProcessDiceSelectionInput(){

        while(true){
            DisplayMenuSelectDice();
            string? input = Console.ReadLine()?.Trim();

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
                Environment.Exit(0);
            }    

            if(int.TryParse(input, out int userDiceIndex) && userDiceIndex >= 0 && userDiceIndex < _diceList.Count){
                userDice = _diceList[userDiceIndex];
                break;
            }

            Console.WriteLine("Invalid input. Please try again.");
        }
    }

    #endregion

    #region DiceSelection

    void ChooseDice()
    {
        if (_playerTurn)
        {
            PlayerChooseDice();
            ComputerChooseDice();
        }
        else
        {
            ComputerChooseDice();
            PlayerChooseDice();
        }
    }

    void PlayerChooseDice()
    {
        ProcessDiceSelectionInput();
        Console.WriteLine($"You selected: {userDice}");
    }

    void ComputerChooseDice()
    {
         // Generate a list of dice excluding the player's choice
        var availableDice = _diceList.Where(dice => dice != userDice).ToList();

        var computerDiceIndex = _random.Next(availableDice.Count);
        computerDice = availableDice[computerDiceIndex];
        Console.WriteLine($"Computer selected: {computerDice}");
    }



    #endregion

    void InicializeGame()
    {
        DisplayIntroduction();

        // Generate HMAC for computer's number
        var (computerNumber, hmac, key) = _randomGenerator.Generate(0, 2);

        Console.WriteLine($"HMAC: {hmac}");

        ProcessInitialTossInput(computerNumber);

        // Calculate result

        Console.WriteLine($"Computer number: {computerNumber}, Key: {BitConverter.ToString(key).Replace('-', '\0')}");
        
    }


    #region GameLogic

     string[] ComputerThrowsDice(){

        Console.WriteLine($"I selected randomNumber between 0...5");
        
        var (computerNumber, hmac, key) = _randomGenerator.Generate(1, 6);
        Console.WriteLine($"HMAC: {hmac}");

        return [computerNumber.ToString(), BitConverter.ToString(key).Replace('-', '\0')];
    }

    string UserThrowsDice(){
        string input = ProcessInputDiceMenu();
        return input;
    }

    static int CalculateRoundResult(int userNumber, int computerNumber){
        Console.WriteLine($"({computerNumber} + {userNumber}) % 6 = {(userNumber + computerNumber) % 6}");
        return (userNumber + computerNumber) % 6;
    }

    void PlayGame(){
        int userThrow = 0, computerThrow = 0;
        string key = "";
        int[] scores = new int[2];
        string[] messages = ["\nIt's your turn", "\nIt's my turn"];
      

        for(int i = 0; i < 2; i++){
            Console.WriteLine(messages[i]);
            string[] computerResult  = ComputerThrowsDice();
            computerThrow = int.Parse(computerResult[0]);
            key = computerResult[1];
            userThrow = int.Parse(UserThrowsDice());
            Console.WriteLine($"Computer number: {computerResult[0]}, Key: {key}");
            scores[i] = CalculateRoundResult(userThrow, computerThrow);
            Console.WriteLine($"Result: {scores[i]}");
        }

        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        int computerScore = computerDice.Sides[scores[1]];
        int userScore = userDice.Sides[scores[0]];
        #pragma warning restore CS8602 // Dereference of a possibly null reference.

       
        Console.WriteLine($"\nYour dice: {userDice} ({userScore}-[{scores[0]}])");
        Console.WriteLine($"Computer dice: {computerDice} ({computerScore}-[{scores[1]}])");
        
        Console.WriteLine("");
        if(computerScore > userScore){
            Console.WriteLine($"({userScore} < {computerScore})");
            Console.WriteLine("Computer wins!");
        } else if(scores[0] == scores[1]){
           Console.WriteLine($"({userScore} = {computerScore})");
           Console.WriteLine("It's a draw!");
        } else{
            Console.WriteLine($"({userScore} > {computerScore})");
            Console.WriteLine("You win!");
        }
        Console.WriteLine("");

    }

    #endregion

    public void StartGame()
    {
        InicializeGame();

        ChooseDice();

        PlayGame();

    }
}
