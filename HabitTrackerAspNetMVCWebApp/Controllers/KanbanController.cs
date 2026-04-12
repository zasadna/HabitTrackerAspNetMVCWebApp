using HabitTrackerAspNetMVCWebApp.Data;
using HabitTrackerAspNetMVCWebApp.Models;
using HabitTrackerAspNetMVCWebApp.Services;
using HabitTrackerAspNetMVCWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HabitTrackerAspNetMVCWebApp.Controllers
{
    [Authorize]
    public class KanbanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HabitScheduleService _scheduleService;

        public KanbanController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _scheduleService = new HabitScheduleService();
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Challenge();
            }

            var habits = await _context.Habits
                .Where(h => h.UserId == currentUser.Id)
                .Include(h => h.HabitLogs)
                .OrderBy(h => h.Title)
                .ToListAsync();

            var viewModel = new KanbanBoardViewModel();

            foreach (var habit in habits)
            {
                var hasLogs = habit.HabitLogs.Any();
                var hasImplicitCompletion = HasImplicitCompletion(habit);
                var hasAnyTrackedActivity = hasLogs || hasImplicitCompletion;
                var hasMissedOccurrences = HasMissedOccurrences(habit);

                if (habit.Status == HabitStatus.Completed)
                {
                    if (hasMissedOccurrences)
                    {
                        viewModel.InProgressHabits.Add(habit);
                    }
                    else
                    {
                        viewModel.DoneHabits.Add(habit);
                    }
                }
                else
                {
                    if (!hasAnyTrackedActivity && !hasMissedOccurrences)
                    {
                        viewModel.TodoHabits.Add(habit);
                    }
                    else
                    {
                        viewModel.InProgressHabits.Add(habit);
                    }
                }
            }

            viewModel.TodoHabits = viewModel.TodoHabits.OrderBy(h => h.Title).ToList();
            viewModel.InProgressHabits = viewModel.InProgressHabits.OrderBy(h => h.Title).ToList();
            viewModel.DoneHabits = viewModel.DoneHabits.OrderBy(h => h.Title).ToList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartToday(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Challenge();
            }

            var habit = await _context.Habits
                .Include(h => h.HabitLogs)
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == currentUser.Id);

            if (habit == null)
            {
                return NotFound();
            }

            var today = DateTime.Today;

            if (!_scheduleService.IsHabitPlannedForDate(habit, today))
            {
                return RedirectToAction(nameof(Index));
            }

            var existingLog = await _context.HabitLogs
                .FirstOrDefaultAsync(hl => hl.HabitId == habit.Id && hl.LogDate.Date == today);

            if (existingLog == null)
            {
                _context.HabitLogs.Add(new HabitLog
                {
                    HabitId = habit.Id,
                    LogDate = today,
                    Status = HabitLogStatus.PartiallyCompleted
                });
            }
            else
            {
                existingLog.Status = HabitLogStatus.PartiallyCompleted;
            }

            if (habit.Status == HabitStatus.Completed)
            {
                habit.Status = HabitStatus.Active;
                habit.EndDate = null;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkTodayComplete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Challenge();
            }

            var habit = await _context.Habits
                .Include(h => h.HabitLogs)
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == currentUser.Id);

            if (habit == null)
            {
                return NotFound();
            }

            var today = DateTime.Today;

            if (!_scheduleService.IsHabitPlannedForDate(habit, today))
            {
                return RedirectToAction(nameof(Index));
            }

            var existingLog = await _context.HabitLogs
                .FirstOrDefaultAsync(hl => hl.HabitId == habit.Id && hl.LogDate.Date == today);

            if (existingLog == null)
            {
                _context.HabitLogs.Add(new HabitLog
                {
                    HabitId = habit.Id,
                    LogDate = today,
                    Status = HabitLogStatus.Completed
                });
            }
            else
            {
                existingLog.Status = HabitLogStatus.Completed;
            }

            if (habit.Status == HabitStatus.Completed)
            {
                habit.Status = HabitStatus.Active;
                habit.EndDate = null;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteHabit(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Challenge();
            }

            var habit = await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == currentUser.Id);

            if (habit == null)
            {
                return NotFound();
            }

            var today = DateTime.Today;

            habit.Status = HabitStatus.Completed;
            habit.EndDate = today;

            if (_scheduleService.IsHabitPlannedForDate(habit, today))
            {
                var existingLog = await _context.HabitLogs
                    .FirstOrDefaultAsync(hl => hl.HabitId == habit.Id && hl.LogDate.Date == today);

                if (existingLog == null)
                {
                    _context.HabitLogs.Add(new HabitLog
                    {
                        HabitId = habit.Id,
                        LogDate = today,
                        Status = HabitLogStatus.Completed
                    });
                }
                else
                {
                    existingLog.Status = HabitLogStatus.Completed;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReopenHabit(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Challenge();
            }

            var habit = await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == currentUser.Id);

            if (habit == null)
            {
                return NotFound();
            }

            habit.Status = HabitStatus.Active;
            habit.EndDate = null;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool HasImplicitCompletion(Habit habit)
        {
            return habit.Status == HabitStatus.Completed && habit.EndDate.HasValue;
        }

        private bool HasMissedOccurrences(Habit habit)
        {
            var today = DateTime.Today;

            DateTime rangeEnd;

            if (habit.Status == HabitStatus.Completed && habit.EndDate.HasValue)
            {
                rangeEnd = habit.EndDate.Value.Date;
            }
            else
            {
                rangeEnd = today;
            }

            if (rangeEnd < habit.StartDate.Date)
            {
                return false;
            }

            var loggedDates = habit.HabitLogs
                .Select(hl => hl.LogDate.Date)
                .Distinct()
                .ToHashSet();

            for (var date = habit.StartDate.Date; date <= rangeEnd; date = date.AddDays(1))
            {
                if (!_scheduleService.IsHabitPlannedForDate(habit, date))
                {
                    continue;
                }

                if (date >= today && habit.Status != HabitStatus.Completed)
                {
                    continue;
                }

                if (loggedDates.Contains(date))
                {
                    continue;
                }

                if (IsImplicitlyCompletedForDate(habit, date))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private bool IsImplicitlyCompletedForDate(Habit habit, DateTime date)
        {
            return habit.Status == HabitStatus.Completed &&
                   habit.EndDate.HasValue &&
                   habit.EndDate.Value.Date == date.Date;
        }
    }
}