using CodingTracker.models;
using CodingTracker.services;

namespace CodingTracker.utils;

internal static class SeedData
{
    internal static void SeedSessions(int count)
    {
        Random random = new();
        DateTime currentDate = DateTime.Now.Date;
        
        List<CodingSession> sessions = new();

        for (int i = 1; i <= count; i++)
        {
            DateTime startDate = currentDate.AddHours(random.Next(13));
            DateTime endDate = startDate.AddHours(random.Next(13));
            
            sessions.Add(new CodingSession
            {
                Id = 1,
                StartTime = startDate,
                EndTime = endDate
            });
            
            currentDate = currentDate.AddDays(1);
        }
        
        var databaseService = new DatabaseService();
        databaseService.BulkInsertSessions(sessions);
    }
}