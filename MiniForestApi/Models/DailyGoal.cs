using System;
using System.ComponentModel.DataAnnotations;

public class DailyGoal
{
    public int Id { get; set; }
    
    [Required]
    public int TargetMinutes { get; set; }
    
    // Hedefin en son ne zaman güncellendiğini takip eder.
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}