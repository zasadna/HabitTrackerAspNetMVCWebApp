using System;
using System.ComponentModel.DataAnnotations;

namespace HabitTrackerAspNetMVCWebApp.Models
{
    public class Habit
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Habit name is required.")]
        [StringLength(100, ErrorMessage = "Habit name cannot exceed 100 characters.")]
        [Display(Name = "Habit Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Completed")]
        public bool IsCompleted { get; set; }

        public string UserId { get; set; }
    }
}