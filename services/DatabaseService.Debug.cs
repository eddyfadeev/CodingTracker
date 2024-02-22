using CodingTracker.models;
using Dapper;

namespace CodingTracker.services;

internal partial class DatabaseService
{
    internal void BulkInsertSessions(List<CodingSession> sessions)
    {
        using var connection = GetConnection();
        
        string query = """
                            INSERT INTO records (StartTime, EndTime, Duration)
                            VALUES (@StartTime, @EndTime, @Duration)
                       """;

        connection.Execute(query, sessions.Select(session => new
        {
            session.StartTime,
            session.EndTime,
            session.Duration
        }));
    }
}