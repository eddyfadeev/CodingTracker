using static CodingTracker.utils.Utilities;
using static CodingTracker.utils.Validation;

using CodingTracker.utils;
using CodingTracker.views;
using Spectre.Console;

namespace CodingTracker.services;

internal class ReportService(DatabaseService databaseService)
{
    private readonly DatabaseService _databaseService = databaseService;
    private Table? _report;
    private readonly uint _monthLimit = 12;
    private readonly uint _yearTopLimit = uint.Parse(DateTime.Today.Year.ToString());
    private readonly uint _yearBottomLimit = uint.Parse(DateTime.Today.Year.ToString()) - 10;
    private readonly int _todayYear = int.Parse(DateTime.Today.Year.ToString());
    private readonly int _todayMonth = int.Parse(DateTime.Today.Month.ToString());

    internal void DateToToday()
    {
        CreateReport(singleDate:true);
        
        AnsiConsole.Write(_report!);
        ContinueMessage();
    }

    internal void DateRange()
    {
        CreateReport(singleDate:false);
        
        AnsiConsole.Write(_report!);
        ContinueMessage();
    }

    internal void Total()
    {
        var controller = new CodingController(_databaseService);
        _report = controller.ViewRecords();

        if (_report is null)
        {
            return;
        }
        
        AnsiConsole.Write(_report);
        SavePrompt(_report);
        ContinueMessage();
    }

    internal void TotalForMonth()
    {
        int month = _todayMonth;
        int year = _todayYear;
        
        try
        {
            month = (int) ValidateNumber("Enter the month number.", _monthLimit);
            year = GetCorrectYear();
        } 
        catch (ExitToMainMenuException e)
        {
            AnsiConsole.WriteLine(e.Message);
            ContinueMessage();
        }
        
        CreateReport(
            singleDate:false,
            startDate: new DateTime(year: year, month: month, day: 01),
            endDate: new DateTime(year: year, month: month, day: DateTime.DaysInMonth(year, month))
            );
        
        if (_report is null)
        {
            return;
        }
        
        AnsiConsole.Write(_report);
        ContinueMessage();
    }

    internal void TotalForYear()
    {
        int year = _todayYear;
        
        try
        {
            year = GetCorrectYear();
        }
        catch (ExitToMainMenuException e)
        {
            AnsiConsole.WriteLine(e.Message);
            ContinueMessage();
        }
        
        CreateReport(
            singleDate:false,
            startDate: new DateTime(year: year, month: 01, day: 01),
            endDate: new DateTime(year: year, month: 12, day: DateTime.DaysInMonth(year, 12))
            );

        if (_report is null)
        {
            return;
        }
        
        AnsiConsole.Write(_report);
        ContinueMessage();
    }
    
    private void CreateReport(bool singleDate, DateTime? startDate = null, DateTime? endDate = null)
    {
        var tableConstructor = new SummaryConstructor();
        UserInput userInput = new();
        DateTime[] dates;

        if (startDate is not null && endDate is not null && !singleDate)
        {
            dates =
            [
                startDate.Value, 
                endDate.Value
            ];
        }
        else if (startDate is not null && !singleDate)
        {
            dates = 
            [
                endDate.Value,
                userInput.GetDateInputs(singleDate: true)[0]
            ];
        }
        else if (startDate is null && singleDate)
        {
            dates = userInput.GetDateInputs(singleDate: true);
        }
        else
        {
            dates = singleDate 
                ? userInput.GetDateInputs(singleDate:true) 
                : userInput.GetDateInputs();
        }
        
        var records = _databaseService.GetCodingSessionsByDate(dates[0], dates[1]);

        var codingSessions = CheckForAnyRecord(records);
        
        if (codingSessions.Count == 0)
        {
            ContinueMessage();
            return;
        }
        
        tableConstructor.PopulateWithRecords(codingSessions);

        _report = tableConstructor.SummaryTable;
    }

    private void SavePrompt(Table table)
    {
        var wantToSave = AnsiConsole.Confirm("Would you like to save this report?");

        if (wantToSave)
        {
            SaveReport(table);
        }
    }
    
    private void SaveReport(Table table)
    {
        var actualTime = DateTime.Now
            .ToString("HH:mm:ss")
            .Split(":")
            .Aggregate((x, y) => x + "-" + y);
        var textWriter = new StringWriter();

        var console = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Out = new AnsiConsoleOutput(textWriter)
        });
        
        console.Write(table);
        console.Write($"Coding Tracker Report. Generated on {DateTime.Now:f}");
        
        File.WriteAllText($"report-{DateTime.Now.Date:dd-MM-yyyy}-{actualTime}.txt", textWriter.ToString());

        AnsiConsole.Console = AnsiConsole.Create(new AnsiConsoleSettings());
        
        AnsiConsole.WriteLine("Save complete.");
        ContinueMessage();
    }
    
    private int GetCorrectYear()
    {
        return (int)ValidateNumber(
            message:"Enter the year.", 
            topLimit:_yearTopLimit, 
            bottomLimit:_yearBottomLimit
        );
    }
}