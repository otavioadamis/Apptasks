using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTOs
{
    public class UserRegisterDTO
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
