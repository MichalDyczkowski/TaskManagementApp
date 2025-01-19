using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApp.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        public string CreatedByUserId { get; set; } = "";

        [ForeignKey("CreatedByUserId")]
        public ApplicationUser? CreatedByUser { get; set; }

        public string AssignedUserId { get; set; } = "";

        [ForeignKey("AssignedUserId")]
        public ApplicationUser? AssignedUser { get; set; }
    }
}
