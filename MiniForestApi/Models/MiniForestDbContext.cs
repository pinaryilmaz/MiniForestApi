using Microsoft.EntityFrameworkCore;
using MiniForestApp.Models; 

namespace MiniForestApp.Models
{
    public class MiniForestDbContext : DbContext
    {
        public MiniForestDbContext(DbContextOptions<MiniForestDbContext> options)
            : base(options)
        {
        }

        public DbSet<FocusSession> FocusSessions { get; set; }
        public DbSet<DailyGoal> DailyGoals { get; set; }
    }
}