using static CodingTracker.utils.Utilities;
using static CodingTracker.utils.Validation;

using CodingTracker.utils;
using CodingTracker.views;
using Spectre.Console;

namespace CodingTracker.services;

internal class ReportService
{
    private readonly DatabaseService _databaseService;
    private Table _report;
    private Table _reportForSaving;
    private string _formattedDuration;
    private readonly uint _monthLimit = 12;
    private readonly uint _yearTopLimit = uint.Parse(DateTime.Today.Year.ToString());
    private readonly uint _yearBottomLimit = uint.Parse(DateTime.Today.Year.ToString()) - 10;
    
    internal ReportService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        _formattedDuration = string.Empty;
        _report = new Table();
        _reportForSaving = new Table();
    }

    internal void DateToToday()
    {
        try
        {
            CreateReport(singleDate:true);
        }
        catch (ReturnBackException)
        {
            ContinueMessage();
            return;
        }
        
        AnsiConsole.Write(_report);
        SavePrompt();
        ContinueMessage();
    }

    internal void DateRange()
    {
        try
        {
            CreateReport(singleDate:false);
        }
        catch (ReturnBackException)
        {
            ContinueMessage();
            return;
        }
        
        AnsiConsole.Write(_report);
        SavePrompt();
        ContinueMessage();
    }

    internal void Total()
    {
        var controller = new CodingController(_databaseService);
        
        var report = controller.PrepareRecords();

        if (report.summaryForRender is null || report.summaryForSave is null)
        {
            return;
        }
        
        _report = report.summaryForRender;
        _reportForSaving = report.summaryForSave;
        
        AnsiConsole.Write(_report);
        SavePrompt();
        ContinueMessage();
    }

    internal void TotalForMonth()
    {
        int month;
        int year;
        try
        {
            month = (int) ValidateNumber("Enter the month number (1-12).", _monthLimit);
            year = GetCorrectYear();
        } 
        catch (ReturnBackException)
        {
            ContinueMessage();
            return;
        }
        
        var result = CreateReport(
            singleDate:false,
            startDate: new DateTime(year: year, month: month, day: 01),
            endDate: new DateTime(year: year, month: month, day: DateTime.DaysInMonth(year, month))
            );

        if (result)
        {
            AnsiConsole.Write(_report);
            SavePrompt();
        }
        
        ContinueMessage();
    }

    internal void TotalForYear()
    {
        int year;
        
        try
        {
            year = GetCorrectYear();
        }
        catch (ReturnBackException)
        {
            ContinueMessage();
            return;
        }
        
        var result = CreateReport(
            singleDate:false,
            startDate: new DateTime(year: year, month: 01, day: 01),
            endDate: new DateTime(year: year, month: 12, day: DateTime.DaysInMonth(year, 12))
            );
        
        if (result)
        {
            AnsiConsole.Write(_report);
            SavePrompt();
        }
        
        ContinueMessage();
    }
    
    private bool CreateReport(bool singleDate, DateTime? startDate = null, DateTime? endDate = null)
    {
        var tableConstructor = new SummaryConstructor();
        DateTime[]? dates;
        
        dates = ProcessDates(singleDate, startDate, endDate);
        
        var records = _databaseService.GetCodingSessionsByDate(dates[0], dates[1]);

        var codingSessions = CheckForAnyRecord(records);
        
        if (codingSessions.Count == 0)
        {
            return false;
        }
        
        tableConstructor.PopulateWithRecords(codingSessions);
        
        _formattedDuration = tableConstructor.FormattedDuration;
        _reportForSaving = tableConstructor.SummaryTableForSaving;
        _report = tableConstructor.SummaryTable;
        
        return true;
    }

    private void SavePrompt()
    {
        var wantsToSave = AnsiConsole.Confirm("\nWould you like to save this report?");

        if (wantsToSave)
        {
            SaveReport();
        }
    }
    
    private void SaveReport()
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
        
        console.Write(_reportForSaving);
        console.Write($"Coding Tracker Report. Generated on {DateTime.Now:f}\n");
        console.Write(_formattedDuration);
        
        File.WriteAllText($"report-{DateTime.Now.Date:dd-MM-yyyy}-{actualTime}.csv", textWriter.ToString());

        AnsiConsole.Console = AnsiConsole.Create(new AnsiConsoleSettings());
        
        AnsiConsole.MarkupLine("\n[green]Save compete[/]");
    }
    
    private int GetCorrectYear()
    {
        return (int)ValidateNumber(
            message:"Enter the year (yyyy).", 
            topLimit:_yearTopLimit, 
            bottomLimit:_yearBottomLimit
        );
    }

    private DateTime[] ProcessDates(bool singleDate, DateTime? startDate, DateTime? endDate)
    {
        UserInput userInput = new();
        DateTime[] processedDates;

        switch (singleDate)
        {
            case true when startDate is null:
                processedDates =
                [
                    userInput.GetDateInputs(singleDate: true)[0],
                    DateTime.Now, 
                ];
                break;
            case false when startDate is null && endDate is null:
                processedDates = userInput.GetDateInputs();
                break;
            case false when startDate is null || endDate is null:
                processedDates =
                [
                    startDate ?? userInput.GetDateInputs(singleDate:true)[0],
                    endDate ?? userInput.GetDateInputs(singleDate:true)[0]
                ];
                break;
            default:
                endDate ??= DateTime.Now; 
                processedDates = 
                [
                    startDate.Value,
                    endDate.Value
                ];
                break;
        }
        
        return processedDates;
    }
}