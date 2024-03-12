using CodingTracker.enums;
using CodingTracker.utils;
using Spectre.Console;

namespace CodingTracker.views;

public static class MenuView
{
    internal static MainMenuEntries? ShowMainMenu()
    {
        var mainMenuEntries = Utilities.GetEnumValuesAndDisplayNames<MainMenuEntries>();

        return ShowMenuPrompt(mainMenuEntries);
    }

    internal static ReportTypes? ShowReportsMenu()
    {
        var reportMenuEntries = Utilities.GetEnumValuesAndDisplayNames<ReportTypes>();
        
        return ShowMenuPrompt(reportMenuEntries);
    }

    internal static TimerMenuEntries? ShowTimerMenu()
    {
        var timerMenuEntries = Utilities.GetEnumValuesAndDisplayNames<TimerMenuEntries>();
        
        return ShowMenuPrompt(timerMenuEntries);
    }
    
    private static TEnum? ShowMenuPrompt<TEnum>(
        IEnumerable<KeyValuePair<TEnum, string>> menuEntries) where TEnum : struct, Enum
    {
        Console.Clear();
        
        if (!menuEntries.Any())
        {
            AnsiConsole.WriteLine($"Problem reading {menuEntries.GetType().Name} entries.");
            return null;
        }
        
        var userChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do?")
                .AddChoices(menuEntries.Select(e => e.Value))
            );
        
        var selectedEntry = menuEntries.SingleOrDefault(e => e.Value == userChoice);

        return selectedEntry.Key;
    }
}