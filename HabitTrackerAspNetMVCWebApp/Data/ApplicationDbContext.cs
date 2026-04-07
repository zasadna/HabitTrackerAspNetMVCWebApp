using HabitTrackerAspNetMVCWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HabitTrackerAspNetMVCWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Habit> Habits { get; set; } = null!;
    }
}