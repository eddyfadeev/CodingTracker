using static CodingTracker.utils.Validation;

using CodingTracker.models;
using CodingTracker.utils;
using CodingTracker.views;
using Spectre.Console;

namespace CodingTracker.services;

/// <summary>
/// This class initializes the database and handles the user's input for CRUD operations. Methods are invoked with
/// reflection, based on the method name passed as custom attribute above corresponding enum entry.
/// </summary>
internal class CodingController 
{
    private readonly DatabaseService _databaseService = new ();
    
    public CodingController()
    {
        _databaseService.InitializeDatabase();
    }

    internal void AddRecord()
    {
        CodingSession session = new();
        UserInput userInput = new();

        var dates = userInput.GetDateInputs();
        
        session.StartTime = dates[0];
        session.EndTime = dates[1];
        
        _databaseService.InsertRecord(session);
    }
    
    private void ViewRecords()
    {
        var tableConstructor = new SummaryConstructor();
        var records = _databaseService.GetAllCodingSessions();
        
        if (records is null)
        {
            AnsiConsole.WriteLine("No records found.");
        }
        else
        {
            var codingSessions = records.ToList();
        
            var table = tableConstructor.ShowRecords(codingSessions);
        
            AnsiConsole.Write(table);
        }
        
        ContinueMessage();
    }
    
    internal void DeleteRecord()
    {
        var userInput = new UserInput();
        var databaseService = new DatabaseService();
        
        ViewRecords();

        var id = userInput.GetIdInput();

        if (!AnsiConsole.Confirm("Are you sure?"))
        {
            return;
        }

        var response = databaseService.DeleteRecord(id);
        
        var responseMessage = response < 1 ? "No record with that ID exists." : "Record deleted successfully.";
        
        AnsiConsole.WriteLine(responseMessage);
        
        ContinueMessage();
    }
    
    internal void UpdateRecord()
    {
        var userInput = new UserInput();
        CodingSession session;
        DateTime[] dates;
        
        var sessions = _databaseService.GetAllCodingSessions();
        
        if (sessions is null)
        {
            ContinueMessage();
            
            return;
        }
        
        try
        {
            var id = userInput.GetIdInput();
            
            session = sessions.Single(x => x.Id == id);
            
            dates = userInput.GetDateInputs();
        }
        catch (InvalidOperationException)
        {
            AnsiConsole.WriteLine("No record with that ID exists.");
            ContinueMessage();

            return;
        }
        catch (ExitToMainMenuException)
        {
            ContinueMessage();
            
            return;
        }
        
        session.StartTime = dates[0];
        session.EndTime = dates[1];
        
        _databaseService.UpdateRecord(session);
    }

    private void ContinueMessage()
    {
        AnsiConsole.WriteLine("Press any key to continue...\n");
        Console.ReadKey();
    }
}