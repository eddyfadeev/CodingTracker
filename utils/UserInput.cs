using Spectre.Console;
using static CodingTracker.utils.Validation;

namespace CodingTracker.utils;

public class UserInput
{
    internal DateTime[] GetDateInputs(bool singleDate = false)
    {
        AnsiConsole.WriteLine("Please enter a start date.");
        var startDate = ValidateDate();
        
        if (singleDate)
        {
            return [startDate];
        }
        
        AnsiConsole.WriteLine("Please enter an end date.");
        var endDate = ValidateDate();

        while (startDate > endDate) 
        {
            AnsiConsole.WriteLine("The end date must be after the start date.");
            endDate = ValidateDate();
        }

        return [startDate, endDate];
    }

    internal int GetIdInput()
    {
        var id = ValidateNumber("Enter the ID of the record.");
        
        return (int)id;
    }
}