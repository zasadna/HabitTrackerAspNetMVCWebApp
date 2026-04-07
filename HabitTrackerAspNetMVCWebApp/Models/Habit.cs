using System;
using System.ComponentModel.DataAnnotations;

namespace HabitTrackerAspNetMVCWebApp.Models
{
    public class Habit
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        public bool IsCompleted { get; set; }
    }
}