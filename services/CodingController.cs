using CodingTracker.models;
using CodingTracker.utils;
using CodingTracker.views;
using Spectre.Console;

namespace CodingTracker.services;

internal class CodingController : ServiceHelpers
{
    private readonly DatabaseService _databaseService = new DatabaseService();
    
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
    
    internal void ViewRecords()
    {
        var tableConstructor = new SummaryConstructor();
        var records = _databaseService.GetAllCodingSessions();
        var table = tableConstructor.ShowAllRecords(records);
        
        AnsiConsole.Write(table);
    }
    
    internal void DeleteRecord()
    {
        // TODO: Implement AddRecord
    }
    
    internal void UpdateRecord()
    {
        // TODO: Implement AddRecord
    }
    
    internal void CreateReport()
    {
        // TODO: Implement AddRecord
    }
}