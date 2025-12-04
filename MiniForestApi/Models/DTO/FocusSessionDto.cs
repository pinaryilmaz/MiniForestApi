using MiniForestApp.Models;

public class FocusSessionDto(FocusSession session)
{
    public int Id { get; set; } = session.Id;
    public int DurationMinutes { get; set; } = session.DurationMinutes;
    public DateTime StartTime { get; set; } = session.StartTime;
    public DateTime? EndTime { get; set; } = session.EndTime;
    public bool IsCompleted { get; set; } = session.IsCompleted;
}
