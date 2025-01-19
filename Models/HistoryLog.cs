using System.ComponentModel.DataAnnotations;

namespace TaskManagementApp.Models
{
    public class HistoryLog
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "";
        public string Action { get; set; } = "";
        [DataType(DataType.DateTime)]
        public DateTime ActionDate { get; set; }
    }
}
