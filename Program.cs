using CodingTracker.enums;
using CodingTracker.services;
using CodingTracker.views;
using Spectre.Console;

namespace CodingTracker;

internal static class Program
{
    private static void Main(string[] args)
    {
        Start();
    }

    private static void Start()
    {
        var controller = new CodingController();
        var isRunning = true;

        do
        {
            var userChoice = MenuView.ShowMainMenu();
            
            if (userChoice == MainMenuEntries.Quit)
            {
                isRunning = false;
                AnsiConsole.WriteLine("Goodbye!");
                
                continue;
            }
            
            CodingController.InvokeActionForMenuEntry(userChoice, controller);
            
        } while(isRunning);
        
    }
}