using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTOs
{
    public class AssignTaskModel
    {
        [Required]
        public string TaskName { get; set; }

        [Required]
        public string UserEmail { get; set; }

    }
}
