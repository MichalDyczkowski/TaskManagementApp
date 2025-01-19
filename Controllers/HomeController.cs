using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Data;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly TaskManagerDbContext _context;

        public HomeController(TaskManagerDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string sortOrder, string search)
        {
            var sevenDaysLater = DateTime.Now.AddDays(7);


            var upcomingTasksQuery = _context.TaskItems
                .Include(task => task.CreatedByUser) 
                .Include(task => task.AssignedUser) 
                .Where(task => !task.IsCompleted && task.DueDate >= DateTime.Now && task.DueDate <= sevenDaysLater);

            if (!string.IsNullOrEmpty(search))
            {
                upcomingTasksQuery = upcomingTasksQuery.Where(task =>
                    task.Title.Contains(search) || task.Description.Contains(search));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    upcomingTasksQuery = upcomingTasksQuery.OrderByDescending(task => task.DueDate);
                    break;
                case "date_asc":
                    upcomingTasksQuery = upcomingTasksQuery.OrderBy(task => task.DueDate);
                    break;
                case "title_asc":
                    upcomingTasksQuery = upcomingTasksQuery.OrderBy(task => task.Title);
                    break;
                case "title_desc":
                    upcomingTasksQuery = upcomingTasksQuery.OrderByDescending(task => task.Title);
                    break;
                default:
                    upcomingTasksQuery = upcomingTasksQuery.OrderBy(task => task.DueDate); 
                    break;
            }

            var upcomingTasks = upcomingTasksQuery.Take(5).ToList(); 

            return View(upcomingTasks);
        }
    }
}
