namespace CodingTracker.models;

internal class CodingSession(DateTime startTime, DateTime endTime)
{
    internal int Id { get; set; }
    internal DateTime StartTime { get; set; } = startTime;
    internal DateTime EndTime { get; set; } = endTime;
    internal TimeSpan Duration => EndTime - StartTime;
}