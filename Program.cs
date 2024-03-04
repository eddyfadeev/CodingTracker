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

            if (userChoice == MainMenuEntries.Reports)
            {
                OpenReportsMenu();
                continue;
            }
            
            ServiceHelpers.InvokeActionForMenuEntry(userChoice, controller);
            
        } while(isRunning);
    }

    private static void OpenReportsMenu()
    {
        var reportsService = new ReportService();
        var isRunning = true;

        do
        {
            var userChoice = ReportsMenuView.ShowReportsMenu();

            if (userChoice == ReportTypes.BackToMainMenu)
            {
                isRunning = false;
                continue;
            }
            
            ServiceHelpers.InvokeActionForMenuEntry(userChoice, reportsService);
            
        } while (isRunning);
    }
}