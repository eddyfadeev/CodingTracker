using static CodingTracker.utils.Utilities;

using CodingTracker.utils;
using CodingTracker.views;
using Spectre.Console;

namespace CodingTracker.services;

internal class ReportService(DatabaseService databaseService)
{
    private readonly DatabaseService _databaseService = databaseService;

    internal void DateToToday()
    {
        var tableConstructor = new SummaryConstructor();
        UserInput userInput = new();

        var dates = userInput.GetDateInputs(singleDate:true);
        
        var records = _databaseService.GetCodingSessionsByDate(dates[0], DateTime.Today);
        
        if (records is null)
        {
            AnsiConsole.WriteLine("No records found.");
        }
        else
        {
            var codingSessions = records.ToList();
            tableConstructor.PopulateWithRecords(codingSessions);

            AnsiConsole.Write(tableConstructor.SummaryTable);
        }
        
        ContinueMessage();
    }
}