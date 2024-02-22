namespace CodingTracker.models;

internal class Goal
{
    internal int Id { get; set; }
    internal TimeSpan DailyGoal { get; set; }
    internal DateTime DateStarted { get; set; }
    internal DateTime DateToAchieveBy { get; set; }
}