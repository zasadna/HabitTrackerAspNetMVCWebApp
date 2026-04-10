using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HabitTrackerAspNetMVCWebApp.Data;
using HabitTrackerAspNetMVCWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HabitTrackerAspNetMVCWebApp.Controllers
{
    [Authorize]
    public class HabitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HabitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();

            var habits = await _context.Habits
                .Where(h => h.UserId == userId)
                .ToListAsync();

            return View(habits);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();

            var habit = await _context.Habits
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (habit == null)
            {
                return NotFound();
            }

            return View(habit);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartDate,IsCompleted")] Habit habit)
        {
            if (!ModelState.IsValid)
            {
                return View(habit);
            }

            habit.UserId = GetCurrentUserId();

            _context.Add(habit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();

            var habit = await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (habit == null)
            {
                return NotFound();
            }

            return View(habit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartDate,IsCompleted")] Habit habit)
        {
            if (id != habit.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(habit);
            }

            var userId = GetCurrentUserId();

            var existingHabit = await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (existingHabit == null)
            {
                return NotFound();
            }

            existingHabit.Name = habit.Name;
            existingHabit.Description = habit.Description;
            existingHabit.StartDate = habit.StartDate;
            existingHabit.IsCompleted = habit.IsCompleted;

            try
            {
                _context.Update(existingHabit);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HabitExists(id, userId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();

            var habit = await _context.Habits
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (habit == null)
            {
                return NotFound();
            }

            return View(habit);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();

            var habit = await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (habit != null)
            {
                _context.Habits.Remove(habit);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HabitExists(int id, string userId)
        {
            return _context.Habits.Any(e => e.Id == id && e.UserId == userId);
        }
    }
}