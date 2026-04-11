using HabitTrackerAspNetMVCWebApp.Data;
using HabitTrackerAspNetMVCWebApp.Models;
using HabitTrackerAspNetMVCWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HabitTrackerAspNetMVCWebApp.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? year, int? month)
        {
            var today = DateTime.Today;
            int selectedYear = year ?? today.Year;
            int selectedMonth = month ?? today.Month;

            var firstDayOfMonth = new DateTime(selectedYear, selectedMonth, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var habits = await _context.Habits
                .Where(h => h.UserId == userId)
                .OrderBy(h => h.Title)
                .ToListAsync();

            var habitLogs = await _context.HabitLogs
                .Include(hl => hl.Habit)
                .Where(hl =>
                    hl.Habit != null &&
                    hl.Habit.UserId == userId &&
                    hl.LogDate.Date >= firstDayOfMonth.Date &&
                    hl.LogDate.Date <= lastDayOfMonth.Date &&
                    hl.IsCompleted)
                .ToListAsync();

            var days = BuildCalendarDays(firstDayOfMonth, lastDayOfMonth, habits, habitLogs);

            var viewModel = new CalendarViewModel
            {
                Year = selectedYear,
                Month = selectedMonth,
                MonthName = firstDayOfMonth.ToString("MMMM yyyy"),
                Days = days,
                PrevMonth = firstDayOfMonth.AddMonths(-1).Month,
                PrevYear = firstDayOfMonth.AddMonths(-1).Year,
                NextMonth = firstDayOfMonth.AddMonths(1).Month,
                NextYear = firstDayOfMonth.AddMonths(1).Year
            };

            return View(viewModel);
        }

        private List<CalendarDayViewModel> BuildCalendarDays(
            DateTime firstDayOfMonth,
            DateTime lastDayOfMonth,
            List<Habit> habits,
            List<HabitLog> habitLogs)
        {
            var result = new List<CalendarDayViewModel>();

            int startOffset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;
            var calendarStart = firstDayOfMonth.AddDays(-startOffset);

            int endOffset = 6 - (((int)lastDayOfMonth.DayOfWeek + 6) % 7);
            var calendarEnd = lastDayOfMonth.AddDays(endOffset);

            for (var date = calendarStart; date <= calendarEnd; date = date.AddDays(1))
            {
                var plannedHabits = habits
                    .Where(h => IsHabitPlannedForDate(h, date))
                    .Select(h => h.Title)
                    .Distinct()
                    .ToList();

                var completedFromLogs = habitLogs
                    .Where(hl => hl.LogDate.Date == date.Date && hl.Habit != null)
                    .Select(hl => hl.Habit!.Title);

                var completedFromHabitStatus = habits
                    .Where(h =>
                        h.Status == HabitStatus.Completed &&
                        h.EndDate.HasValue &&
                        h.EndDate.Value.Date == date.Date)
                    .Select(h => h.Title);

                var completedHabits = completedFromLogs
                    .Concat(completedFromHabitStatus)
                    .Distinct()
                    .ToList();

                result.Add(new CalendarDayViewModel
                {
                    Date = date,
                    IsCurrentMonth = date.Month == firstDayOfMonth.Month,
                    IsToday = date.Date == DateTime.Today,
                    PlannedHabits = plannedHabits,
                    CompletedHabits = completedHabits
                });
            }

            return result;
        }

        private bool IsHabitPlannedForDate(Habit habit, DateTime date)
        {
            if (habit.StartDate.Date > date.Date)
                return false;

           if (habit.EndDate.HasValue && date.Date >= habit.EndDate.Value.Date && habit.Status == HabitStatus.Completed)
                return false;

            switch (habit.Frequency)
            {
                case Frequency.Daily:
                    return true;

                case Frequency.Weekly:
                    return habit.StartDate.DayOfWeek == date.DayOfWeek;

                case Frequency.Monthly:
                    return habit.StartDate.Day == date.Day;

                default:
                    return false;
            }
        }
    }
}