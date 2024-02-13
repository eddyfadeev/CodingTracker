using CodingTracker.models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CodingTracker.services;

internal class DatabaseService
{
    private readonly string _connectionString = AppConfig.GetConnectionString();

    internal void CreateDatabase()
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
    }

    internal List<CodingSession>? GetAllCodingSessions()
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
    
    private SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}