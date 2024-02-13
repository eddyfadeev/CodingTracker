namespace CodingTracker.models;

internal class Goal
{
    internal int Id { get; set; }
    internal TimeSpan DailyGoal { get; set; }
    internal DateTime dateStarted { get; set; }
    internal DateTime dateToAcheiveBy { get; set; }
}