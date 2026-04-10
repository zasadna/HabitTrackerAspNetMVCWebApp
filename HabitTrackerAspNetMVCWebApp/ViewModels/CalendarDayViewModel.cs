using System;
using System.Collections.Generic;

namespace HabitTrackerAspNetMVCWebApp.ViewModels
{
    public class CalendarDayViewModel
    {
        public DateTime Date { get; set; }

        public bool IsCurrentMonth { get; set; }

        public bool IsToday { get; set; }

        public List<string> PlannedHabits { get; set; } = new List<string>();

        public List<string> CompletedHabits { get; set; } = new List<string>();
    }
}