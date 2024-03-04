using CodingTracker.models;
using CodingTracker.utils;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CodingTracker.services;

/// <summary>
/// Provides methods for interacting with the database and performing CRUD operations on coding sessions.
/// </summary>
internal partial class DatabaseService
{
    /// <summary>
    /// Represents the connection string used to connect to the database.
    /// </summary>
    private readonly string _connectionString = AppConfig.GetConnectionString();

    /// <summary>
    /// Initializes the database by creating the necessary table if it doesn't exist and optionally seeds it with sample data in debug mode.
    /// </summary>
    /// <remarks>
    /// This method should be called before any other CRUD operations are performed on the database.
    /// </remarks>
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

    /// <summary>
    /// Retrieves all coding sessions from the database.
    /// </summary>
    /// <returns>
    /// An IEnumerable of CodingSession objects representing the retrieved coding sessions.
    /// </returns>
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

    /// <summary>
    /// Retrieves a list of coding sessions from the database within the specified date range.
    /// </summary>
    /// <param name="start">The start date and time of the range.</param>
    /// <param name="end">The end date and time of the range.</param>
    /// <returns>A list of coding sessions that fall within the specified date range.</returns>
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

    /// <summary>
    /// Inserts a coding session record into the database.
    /// </summary>
    /// <param name="session">The coding session to be inserted.</param>
    internal void InsertRecord(CodingSession session)
    {
        using var connection = GetConnection(); 
        
        string insertQuery = """
                             
                                INSERT INTO records (StartTime, EndTime, Duration)
                                VALUES (@StartTime, @EndTime, @Duration)
                             """;
        
        connection.Execute(insertQuery, new { session.StartTime, session.EndTime, session.Duration });
    }

    /// <summary>
    /// Updates the specified coding session record in the database with the new start time, end time, and duration.
    /// </summary>
    /// <param name="session">The coding session to update.</param>
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

    /// <summary>
    /// Deletes a record from the database based on the specified record ID.
    /// </summary>
    /// <param name="recordId">The ID of the record to delete.</param>
    /// <returns>The number of records deleted.</returns>
    internal int DeleteRecord(int recordId)
    {
        using var connection = GetConnection();
        
        string query = "DELETE FROM records WHERE Id = @Id";
        
        return connection.Execute(query, new { Id = recordId });
    }

    /// <summary>
    /// Retrieves a connection to the database.
    /// </summary>
    /// <returns>A <see cref="SqliteConnection"/> object representing a connection to the database.</returns>
    private SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        return connection;
    }
}