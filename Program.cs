using CodingTracker.enums;
using CodingTracker.services;
using CodingTracker.views;
using Spectre.Console;

namespace CodingTracker;

internal static class Program
{
    private static readonly DatabaseService _databaseService = new();
    static Program()
    {
        _databaseService.InitializeDatabase();
    }
    
    private static void Main(string[] args)
    {
        Start();
    }

    private static void Start()
    {
        var controller = new CodingController(_databaseService);
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

            if (userChoice == MainMenuEntries.Reports)
            {
                OpenReportsMenu();
                continue;
            }

            if (userChoice == MainMenuEntries.Timer)
            {
                OpenTimerMenu();
            }
            
            ServiceHelpers.InvokeActionForMenuEntry(userChoice, controller);
            
        } while(isRunning);
    }

    private static void OpenReportsMenu()
    {
        var reportsService = new ReportService(_databaseService);
        var isRunning = true;

        do
        {
            var userChoice = MenuView.ShowReportsMenu();

            if (userChoice == ReportTypes.BackToMainMenu)
            {
                isRunning = false;
                continue;
            }
            
            ServiceHelpers.InvokeActionForMenuEntry(userChoice, reportsService);
            
        } while (isRunning);
    }

    private static void OpenTimerMenu()
    {
        var timerService = new TimerService(_databaseService);
        var isRunning = true;

        do
        {
            var userChoice = MenuView.ShowTimerMenu();
            
            if (userChoice == TimerMenuEntries.BackToMainMenu)
            {
                isRunning = false;
                continue;
            }
            
            ServiceHelpers.InvokeActionForMenuEntry(userChoice, timerService);
        } while (isRunning);
    }
}