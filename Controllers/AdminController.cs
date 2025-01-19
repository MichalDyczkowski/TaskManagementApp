using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TaskManagementApp.Data;
using TaskManagementApp.Models;

namespace TaskManagementApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TaskManagerDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager,
                               TaskManagerDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var historyLog = new HistoryLog
                {
                    UserName = User.Identity.Name,
                    Action = $"Usunięto użytkownika: {user.UserName}",
                    ActionDate = DateTime.Now
                };
                _context.HistoryLogs.Add(historyLog);
                await _context.SaveChangesAsync();

                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(ManageUsers));
        }

        public async Task<IActionResult> Logs()
        {
            var logs = await _context.HistoryLogs
                .OrderByDescending(l => l.ActionDate)
                .ToListAsync();
            return View(logs);
        }

        public IActionResult Reports()
        {
            ViewData["Report"] = "Raport przykładowy.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateCsvReport(DateTime startDate, DateTime endDate, string actionType)
        {
            var logs = _context.HistoryLogs
                .Where(log => log.ActionDate >= startDate && log.ActionDate <= endDate);

            Console.WriteLine("Logi przed filtrowaniem:");
            foreach (var log in await logs.ToListAsync())
            {
                Console.WriteLine(log.Action);
            }

            if (!string.IsNullOrEmpty(actionType))
            {
                logs = logs.Where(log => log.Action.Contains(actionType));

                Console.WriteLine($"Filtrujemy logi po akcjach zawierających: {actionType}");
            }

            Console.WriteLine("Logi po filtrowaniu:");
            foreach (var log in await logs.ToListAsync())
            {
                Console.WriteLine(log.Action);
            }

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("UserName,Action,ActionDate");

            foreach (var log in await logs.ToListAsync())
            {
                csvBuilder.AppendLine($"{log.UserName},{log.Action},{log.ActionDate:yyyy-MM-dd HH:mm:ss}");
            }

            var csvData = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            var fileName = $"Report_{DateTime.Now:yyyyMMddHHmmss}.csv";

            return File(csvData, "text/csv", fileName);
        }


    }

}
