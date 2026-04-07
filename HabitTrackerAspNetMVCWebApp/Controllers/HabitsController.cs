using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HabitTrackerAspNetMVCWebApp.Models;
using HabitTrackerAspNetMVCWebApp.Data;

namespace HabitTrackerAspNetMVCWebApp.Controllers
{
    public class HabitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HabitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Habits
        public IActionResult Index()
        {
            var habits = _context.Habits.ToList();
            return View(habits);
        }

        // GET: Habits/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habit = _context.Habits.FirstOrDefault(h => h.Id == id);

            if (habit == null)
            {
                return NotFound();
            }

            return View(habit);
        }

        // GET: Habits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Habits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Habit habit)
        {
            if (ModelState.IsValid)
            {
                _context.Habits.Add(habit);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(habit);
        }

        // GET: Habits/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habit = _context.Habits.Find(id);

            if (habit == null)
            {
                return NotFound();
            }

            return View(habit);
        }

        // POST: Habits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Habit habit)
        {
            if (id != habit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Habits.Update(habit);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(habit);
        }

        // GET: Habits/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habit = _context.Habits.FirstOrDefault(h => h.Id == id);

            if (habit == null)
            {
                return NotFound();
            }

            return View(habit);
        }

        // POST: Habits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var habit = _context.Habits.Find(id);

            if (habit != null)
            {
                _context.Habits.Remove(habit);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}