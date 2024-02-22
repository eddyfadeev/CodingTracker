﻿using Spectre.Console;
using static CodingTracker.utils.Validation;
namespace CodingTracker.utils;

public class UserInput
{
    internal DateTime[] GetDateInputs()
    {
        AnsiConsole.WriteLine("Please enter a start date.");
        var startDate = ValidateDate();
        
        AnsiConsole.WriteLine("Please enter an end date.");
        var endDate = ValidateDate();

        while (startDate > endDate) 
        {
            AnsiConsole.WriteLine("The end date must be after the start date.");
            endDate = ValidateDate();
        }

        return [startDate, endDate];
    }
}