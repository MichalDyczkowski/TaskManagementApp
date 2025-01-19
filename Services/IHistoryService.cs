using TaskManagementApp.Data;
using TaskManagementApp.Models;

namespace TaskManagementApp.Services
{
    public interface IHistoryService
    {
        Task LogActionAsync(string userName, string action);
    }

    public class HistoryService : IHistoryService
    {
        private readonly TaskManagerDbContext _context;

        public HistoryService(TaskManagerDbContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(string userName, string action)
        {
            var log = new HistoryLog
            {
                UserName = userName,
                Action = action,
                ActionDate = DateTime.Now
            };
            _context.HistoryLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
