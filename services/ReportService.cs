using static CodingTracker.utils.Utilities;
using static CodingTracker.utils.Validation;

using CodingTracker.utils;
using CodingTracker.views;
using Spectre.Console;

namespace CodingTracker.services;

/// <summary>
/// This handles the user's input for reporting. Methods are invoked with
/// reflection, based on the method name passed as custom attribute above corresponding enum entry.
/// </summary>
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
        bool result;
        
        try
        {
            result = CreateReport(singleDate:true);
        }
        catch (ReturnBackException)
        {
            ContinueMessage();
            return;
        }
        
        if (result)
        {
            AnsiConsole.Write(_report);
            SavePrompt();
        }
        
        ContinueMessage();
    }

    internal void DateRange()
    {
        bool result;
        
        try
        {
            result = CreateReport(singleDate:false);
        }
        catch (ReturnBackException)
        {
            ContinueMessage();
            return;
        }
        
        if (result)
        {
            AnsiConsole.Write(_report);
            SavePrompt();
        }
        
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

    /// <summary>
    /// Creates a report based on the provided parameters.
    /// </summary>
    /// <param name="singleDate">A boolean value indicating whether the report is for a single date or a date range.</param>
    /// <param name="startDate">The start date of the report. Nullable if singleDate is true.</param>
    /// <param name="endDate">The end date of the report. Nullable if singleDate is true.</param>
    /// <returns>A boolean value indicating whether the report was successfully created.</returns>
    private bool CreateReport(bool singleDate, DateTime? startDate = null, DateTime? endDate = null)
    {
        var tableConstructor = new SummaryConstructor();
        DateTime[]? dates;
        
        dates = ProcessDates(singleDate, startDate, endDate);
        
        var records = _databaseService.RetrieveCodingSessions(dates[0], dates[1]);

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

    /// <summary>
    /// Saves the report generated by the ReportService.
    /// The report is saved as a CSV file with a timestamp in the filename.
    /// </summary>
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

    /// <summary>
    /// Gets the correct year input from the user. The year must be a valid four-digit year
    /// within the limits specified by the ReportService.
    /// </summary>
    /// <returns>The validated year as an integer.</returns>
    private int GetCorrectYear()
    {
        return (int)ValidateNumber(
            message:"Enter the year (yyyy).", 
            topLimit:_yearTopLimit, 
            bottomLimit:_yearBottomLimit
        );
    }

    /// <summary>
    /// Process the dates based on the given parameters.
    /// </summary>
    /// <param name="singleDate">True if a single date is selected, false otherwise.</param>
    /// <param name="startDate">The start date of the date range, null if not applicable.</param>
    /// <param name="endDate">The end date of the date range, null if not applicable.</param>
    /// <returns>An array of DateTime containing the processed dates.</returns>
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