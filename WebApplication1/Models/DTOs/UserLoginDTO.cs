using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTOs
{
    public class UserLoginDTO
    {
            [Required]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }
        
    }
}
