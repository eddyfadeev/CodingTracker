using CodingTracker.models;
using CodingTracker.utils;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CodingTracker.services;

internal partial class DatabaseService
{
    private readonly string _connectionString = AppConfig.GetConnectionString();

    internal void InitializeDatabase()
    {
        using var connection = GetConnection();
        
        const string createTableQuery = """
                                            CREATE TABLE IF NOT EXISTS records (
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            StartTime DATETIME NOT NULL,
                                            EndTime DATETIME NOT NULL,
                                            Duration INTEGER NOT NULL
                                        )
                                        """;

        connection.Execute(createTableQuery);
        
        #if DEBUG
        SeedData.SeedSessions(20);
        #endif
    }

    internal IEnumerable<CodingSession>? GetAllCodingSessions()
    {
        try
        {
            using var connection = GetConnection();
            
            return connection.Query<CodingSession>("SELECT * FROM records").ToList();
        }
        catch (SqliteException e)
        {
            Console.WriteLine("Error getting all coding sessions: " + e.Message);
            return null;
        }
    }

    internal List<CodingSession>? GetCodingSessionsByDate(DateTime start, DateTime end)
    {
        try
        {
            var connection = GetConnection();
            return connection.Query<CodingSession>(
                "SELECT * FROM records WHERE StartTime >= @Start AND EndTime <= @End",
                new { Start = start, End = end }).ToList();
        }
        catch (SqliteException e)
        {
            Console.WriteLine("Error getting coding sessions by date: " + e.Message);
            return null;
        }
    }

    internal void InsertRecord(CodingSession session)
    {
        using var connection = GetConnection(); 
        
        string insertQuery = """
                             
                                INSERT INTO records (StartTime, EndTime, Duration)
                                VALUES (@StartTime, @EndTime, @Duration)
                             """;
        
        connection.Execute(insertQuery, new { session.StartTime, session.EndTime, session.Duration });
    }

    internal void UpdateRecord(CodingSession session)
    {
        using var connection = GetConnection();
        
        string query = """
                            UPDATE records
                            SET StartTime = @StartTime, EndTime = @EndTime, Duration = @Duration
                            WHERE Id = @Id
                        """;
        
        connection.Execute(query, new { session.StartTime, session.EndTime, session.Duration, session.Id });
    }
    
    private SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        return connection;
    }
}