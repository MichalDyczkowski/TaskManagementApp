using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementApp.Data;
using TaskManagementApp.Models;

namespace TaskManagementApp.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly TaskManagerDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(TaskManagerDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string sortOrder, string search)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();

            IQueryable<TaskItem> tasksQuery;
            if (User.IsInRole("Administrator"))
            {
                tasksQuery = _context.TaskItems.Include(t => t.AssignedUser);
            }
            else
            {
                tasksQuery = _context.TaskItems
                    .Where(t => t.AssignedUserId == currentUser.Id)
                    .Include(t => t.AssignedUser);
            }

            if (!string.IsNullOrEmpty(search))
            {
                tasksQuery = tasksQuery.Where(t => t.Title.Contains(search) || t.Description.Contains(search));
            }

            switch (sortOrder)
            {
                case "date_asc":
                    tasksQuery = tasksQuery.OrderBy(t => t.DueDate);
                    break;
                case "date_desc":
                    tasksQuery = tasksQuery.OrderByDescending(t => t.DueDate);
                    break;
                case "title_asc":
                    tasksQuery = tasksQuery.OrderBy(t => t.Title);
                    break;
                case "title_desc":
                    tasksQuery = tasksQuery.OrderByDescending(t => t.Title);
                    break;
                default:
                    tasksQuery = tasksQuery.OrderBy(t => t.DueDate);
                    break;
            }

            var tasks = await tasksQuery.ToListAsync();
            return View(tasks);
        }

        public IActionResult Create()
        {
            if (User.IsInRole("Administrator"))
            {
                var users = _userManager.Users.ToList();
                ViewData["Users"] = users;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItem model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();

            model.CreatedByUserId = currentUser.Id;
            if (string.IsNullOrEmpty(model.AssignedUserId))
                model.AssignedUserId = currentUser.Id;

            var historyLog = new HistoryLog
            {
                UserName = currentUser.UserName,
                Action = $"Dodano zadanie: {model.Title}",
                ActionDate = DateTime.Now
            };
            _context.HistoryLogs.Add(historyLog);

            _context.TaskItems.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var taskItem = await _context.TaskItems
                .Include(t => t.AssignedUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (taskItem == null)
                return NotFound();

            var users = await _userManager.Users.ToListAsync();
            ViewData["Users"] = users;

            return View(taskItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskItem model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var existing = await _context.TaskItems.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.DueDate = model.DueDate;
            existing.IsCompleted = model.IsCompleted;

            if (User.IsInRole("Administrator"))
            {
                if (!string.IsNullOrEmpty(model.AssignedUserId))
                {
                    existing.AssignedUserId = model.AssignedUserId;
                }
            }
            else
            {
                if (model.AssignedUserId != existing.AssignedUserId)
                {
                    ModelState.AddModelError("AssignedUserId", "Nie możesz przypisać zadania do innego użytkownika.");
                    return View("~/Views/Account/AccessDenied.cshtml");
                }
            }

            var historyLog = new HistoryLog
            {
                UserName = User.Identity.Name,
                Action = $"Edytowano zadanie: {model.Title}",
                ActionDate = DateTime.Now
            };
            _context.HistoryLogs.Add(historyLog);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var taskItem = await _context.TaskItems
                .Include(t => t.AssignedUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (taskItem == null)
                return NotFound();

            if (User.IsInRole("Administrator"))
            {
                return View(taskItem);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (taskItem.CreatedByUserId != currentUser.Id && taskItem.AssignedUserId != currentUser.Id)
            {
                return Forbid();
            }

            return View(taskItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await _context.TaskItems
                .Include(t => t.AssignedUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (taskItem != null)
            {
                if (User.IsInRole("Administrator"))
                {
                    _context.TaskItems.Remove(taskItem);
                }
                else
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (taskItem.CreatedByUserId != currentUser.Id && taskItem.AssignedUserId != currentUser.Id)
                    {
                        return Forbid();
                    }
                    _context.TaskItems.Remove(taskItem);
                }

                var currentUserLog = await _userManager.GetUserAsync(User);
                var historyLog = new HistoryLog
                {
                    UserName = currentUserLog.UserName,
                    Action = $"Usunięto zadanie: {taskItem.Title}",
                    ActionDate = DateTime.Now
                };
                _context.HistoryLogs.Add(historyLog);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
