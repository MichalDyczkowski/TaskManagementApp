using TaskManagementApp.Models;

namespace TaskManagementApp.Services
{
    public interface INotificationService
    {
        Task SendAssignmentNotification(TaskItem task);
        Task SendDeadlineNotification(TaskItem task);
    }

    public class NotificationService : INotificationService
    {
        public Task SendAssignmentNotification(TaskItem task)
        {
            return Task.CompletedTask;
        }

        public Task SendDeadlineNotification(TaskItem task)
        {
            return Task.CompletedTask;
        }
    }
}
