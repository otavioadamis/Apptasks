using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTOs
{
    public class UserUpdateDTO
    {

        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

    }
}
