using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using MiniForestApp.Models; 

[ApiController]
[Route("[controller]")] 
public class GoalController : ControllerBase
{
    private readonly MiniForestDbContext _context;
    public GoalController(MiniForestDbContext context) 
    {
        _context = context;
    }

    // POST /Goal/set-goal - Günlük çalışma hedefini belirler
    [HttpPost("set-goal")]
    public async Task<IActionResult> SetGoal([FromBody] SetGoalDto dto)
    {
        try
        {
            var goal = await _context.DailyGoals.FirstOrDefaultAsync();

            if (goal == null)
            {
                goal = new DailyGoal
                {
                    TargetMinutes = dto.TargetMinutes,
                    LastUpdated = DateTime.UtcNow
                };
                _context.DailyGoals.Add(goal);
            }
            else
            {
                goal.TargetMinutes = dto.TargetMinutes;
                goal.LastUpdated = DateTime.UtcNow;
                _context.DailyGoals.Update(goal);
            }

            await _context.SaveChangesAsync();
            return Ok(Response<DailyGoal>.Successful(goal));
        }
        catch (Exception)
        {
            return BadRequest(Response<DailyGoal>.Fail("Hedef belirleme sırasında hata oluştu."));
        }
    }

    // GET /Goal/summary - Güncel hedefi ve bugünün özetini birleştirir
    [HttpGet("summary")]
    public async Task<IActionResult> GetGoalSummary()
    {
        var goal = await _context.DailyGoals.FirstOrDefaultAsync();
        
        // Eğer hedef yoksa, varsayılan bir değer kullan
        int target = goal?.TargetMinutes ?? 120;
        
        // Bugünün toplam süresini alın (FocusController'dan gerekli olan kısmı buraya taşıdık)
        var today = DateTime.UtcNow.Date;
        var total = await _context.FocusSessions
            .Where(x => x.StartTime.Date == today && x.IsCompleted)
            .SumAsync(x => x.DurationMinutes); 

        // Yüzde hesaplama
        double percentage = (total / (double)target) * 100;
        
        var summary = new
        {
            TargetMinutes = target,
            TotalMinutesToday = total,
            ProgressPercentage = Math.Min(100, Math.Round(percentage, 1))
        };

        return Ok(Response<object>.Successful(summary));
    }
}