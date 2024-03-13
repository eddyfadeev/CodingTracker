using static CodingTracker.utils.Validation;
using static CodingTracker.utils.Utilities;

using CodingTracker.models;
using CodingTracker.utils;
using CodingTracker.views;
using Spectre.Console;

namespace CodingTracker.services;

/// <summary>
/// This class initializes the database and handles the user's input for CRUD operations. Methods are invoked with
/// reflection, based on the method name passed as custom attribute above corresponding enum entry.
/// </summary>
internal class CodingController(DatabaseService databaseService) 
{
    private readonly DatabaseService _databaseService = databaseService;

    internal void ViewRecords()
    {
        AnsiConsole.Write(PrepareRecords().summaryForRender);
        ContinueMessage();
    }

    internal void AddRecord()
    {
        CodingSession session = new();
        UserInput userInput = new();
        DateTime[] dates;

        try
        {
            dates = userInput.GetDateInputs();
        }
        catch (ReturnBackException)
        {
            ContinueMessage();
            return;
        }
        
        session.StartTime = dates[0];
        session.EndTime = dates[1];
        
        _databaseService.InsertRecord(session);
    }
    
    internal (Table? summaryForRender, Table? summaryForSave) PrepareRecords()
    {
        var tableConstructor = new SummaryConstructor();
        var records = _databaseService.GetAllCodingSessions();
        
        if (records is null)
        {
            AnsiConsole.WriteLine("No records found.");
            ContinueMessage();
            
            return (null, null);
        }

        var codingSessions = records.ToList();
        tableConstructor.PopulateWithRecords(codingSessions);

        return (tableConstructor.SummaryTable, tableConstructor.SummaryTableForSaving);
    }
    
    internal void DeleteRecord()
    {
        int id;
        var userInput = new UserInput();
        
        AnsiConsole.Write(PrepareRecords().summaryForRender);
        
        try
        {
            id = userInput.GetIdInput();
        }
        catch (ReturnBackException e)
        {
            AnsiConsole.WriteLine(e.Message);
            ContinueMessage();
            return;
        }
        
        if (!AnsiConsole.Confirm("Are you sure?"))
        {
            return;
        }

        var response = _databaseService.DeleteRecord(id);
        
        var responseMessage = response < 1 ? "No record with that ID exists." : "Record deleted successfully.";
        
        AnsiConsole.WriteLine(responseMessage);
        
        ContinueMessage();
    }
    
    internal void UpdateRecord()
    {
        var userInput = new UserInput();
        DateTime[] dates;
        int id;
        CodingSession session;
        
        var sessions = _databaseService.GetAllCodingSessions();
        
        if (sessions is null)
        {
            ContinueMessage();
            
            return;
        }
        
        AnsiConsole.Write(PrepareRecords().summaryForRender);

        try
        {
            id = userInput.GetIdInput();
            session = sessions.Single(x => x.Id == id);
            dates = userInput.GetDateInputs();
        }
        catch (ReturnBackException e)
        {
            AnsiConsole.WriteLine(e.Message);
            ContinueMessage();

            return;
        }
        catch (InvalidOperationException)
        {
            AnsiConsole.WriteLine("\nNo record with that ID exists.");
            ContinueMessage();

            return;
        }
        
        session.StartTime = dates[0];
        session.EndTime = dates[1];
        
        _databaseService.UpdateRecord(session);
        
        AnsiConsole.WriteLine("Record deleted successfully.");
        ContinueMessage();
    }
}