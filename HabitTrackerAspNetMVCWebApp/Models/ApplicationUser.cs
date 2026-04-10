using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HabitTrackerAspNetMVCWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Habit> Habits { get; set; } = new List<Habit>();
    }
}