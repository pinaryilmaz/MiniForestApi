using System.ComponentModel.DataAnnotations;

public class SetGoalDto
{
    [Required(ErrorMessage = "Hedef dakika zorunludur.")]
    [Range(1, 1440, ErrorMessage = "Hedef 1 ile 1440 dakika arasında olmalıdır.")]
    public int TargetMinutes { get; set; }
}