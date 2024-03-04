using CodingTracker.enums;
using CodingTracker.utils;
using Spectre.Console;

namespace CodingTracker.views;

public class ReportsMenuView
{
    internal static ReportTypes? ShowReportsMenu()
    {
        Console.Clear();
        
        var reportMenuEntries = Utilities.GetEnumValuesAndDisplayNames<ReportTypes>();

        if (!reportMenuEntries.Any())
        {
            AnsiConsole.WriteLine("Problem reading report menu entries. Returning to the main menu...");
            return null;
        }

        var userChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What report would you like to view?")
                .AddChoices(reportMenuEntries.Select(e => e.Value))
            );

        var selectedEntry = reportMenuEntries.SingleOrDefault(e => e.Value == userChoice);
        
        return selectedEntry.Key;
    }
}