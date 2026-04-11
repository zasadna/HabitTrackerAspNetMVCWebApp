using HabitTrackerAspNetMVCWebApp.Data;
using HabitTrackerAspNetMVCWebApp.Models;
using HabitTrackerAspNetMVCWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HabitTrackerAspNetMVCWebApp.Controllers
{
    [Authorize]
    public class KanbanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public KanbanController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                .OrderBy(h => h.Title)
                .ToListAsync();

            var viewModel = new KanbanBoardViewModel
            {
                TodoHabits = habits.Where(h => h.KanbanStatus == KanbanStatus.Todo).ToList(),
                InProgressHabits = habits.Where(h => h.KanbanStatus == KanbanStatus.InProgress).ToList(),
                DoneHabits = habits.Where(h => h.KanbanStatus == KanbanStatus.Done).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToTodo(int id)
        {
            return await UpdateHabitKanbanStatus(id, KanbanStatus.Todo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToInProgress(int id)
        {
            return await UpdateHabitKanbanStatus(id, KanbanStatus.InProgress);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToDone(int id)
        {
            return await UpdateHabitKanbanStatus(id, KanbanStatus.Done);
        }

        private async Task<IActionResult> UpdateHabitKanbanStatus(int id, KanbanStatus newKanbanStatus)
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

            habit.KanbanStatus = newKanbanStatus;

            switch (newKanbanStatus)
            {
                case KanbanStatus.Todo:
                case KanbanStatus.InProgress:
                    if (habit.Status == HabitStatus.Completed)
                    {
                        habit.Status = HabitStatus.Active;
                    }

                    habit.EndDate = null;
                    break;

                case KanbanStatus.Done:
                    habit.Status = HabitStatus.Completed;
                    habit.EndDate = DateTime.Today;
                    break;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}