using CodingTracker.enums;
using CodingTracker.utils;
using Spectre.Console;

namespace CodingTracker.views;

public static class MenuView
{
    internal static string? ShowMainMenu()
    {
        var availableChoicesStrings = Utilities.GetMenuEntries(typeof(MainMenuEntries));
        
        if (availableChoicesStrings is null)
        {
            AnsiConsole.WriteLine("Problem reading main menu entries. Exiting...");
            return null;
        }

        var userChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do?")!
                .AddChoices(availableChoicesStrings)
            );

        return userChoice;
    }
}