using System.Globalization;
using Spectre.Console;

namespace CodingTracker.utils;

public static class Validation
{
    public sealed class ExitToMainMenuException(string message = "\nExiting to main menu.") : Exception(message);
    
    internal static DateTime ValidateDate(string message = "Enter the date in the format: dd-mm-yy hh:mm (24h clock).")
    {
        DateTime dateValue;
        bool isValid;

        do
        {
            var input = AnsiConsole.Ask<string>(message);

            try
            {
                if (!string.IsNullOrWhiteSpace(input))
                {
                    CheckForZero(input);
                }

            }
            catch (ExitToMainMenuException e)
            {
                AnsiConsole.WriteLine(e.Message);
                throw;
            }

            isValid = DateTime.TryParseExact(
                input, "dd-MM-yy HH:mm", 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.None, 
                out dateValue
                ) && dateValue < DateTime.Now && dateValue > DateTime.Now.AddYears(-10);

            if (!isValid)
            {
                AnsiConsole.WriteLine("Invalid input or future date. Please enter a date in the past in the format:\n" +
                                      "dd-mm-yy hh:mm (24h clock) and no more than 10 year ago.\n" +
                                      "Enter 0 'zero' to exit to main menu.");
            }

        } while (!isValid);

        return dateValue;
    }

    internal static uint ValidateNumber(string message = "Enter a positive number.", uint topLimit = uint.MaxValue, uint bottomLimit = 0)
    {
        uint number;
        bool isValid;

        do
        {
            var input = AnsiConsole.Ask<string>(message);

            try
            {
                if (!string.IsNullOrWhiteSpace(input))
                {
                    CheckForZero(input);
                }
            }
            catch (ExitToMainMenuException e)
            {
                AnsiConsole.WriteLine(e.Message);
                throw;
            }
            
            isValid = uint.TryParse(input, out number) && number <= topLimit && number >= bottomLimit;
            
            if (!isValid)
            {
                AnsiConsole.WriteLine("Invalid input. Please enter a positive number or 0 to exit to main menu.");
            }
            
        } while(!isValid);

        return number;
    }

    private static void CheckForZero(string input)
    {
        if (input.Equals("0"))
        {
            throw new ExitToMainMenuException();
        }
    }
}