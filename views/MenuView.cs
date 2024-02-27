using System.Collections;
using System.ComponentModel.DataAnnotations;
using CodingTracker.enums;
using CodingTracker.utils;
using Spectre.Console;

namespace CodingTracker.views;

public static class MenuView
{
    internal static MainMenuEntries? ShowMainMenu()
    {
        Console.Clear();
        
        var mainMenuEntries = Utilities.GetEnumValuesAndDisplayNames<MainMenuEntries>();
        
        if (!mainMenuEntries.Any())
        {
            AnsiConsole.WriteLine("Problem reading main menu entries. Exiting...");
            return null;
        }

        var userChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do?")
                .AddChoices(mainMenuEntries.Select(e => e.Value))
            );

        var selectedEntry = mainMenuEntries.SingleOrDefault(e => e.Value == userChoice);
        
        return selectedEntry.Key;
    }
}