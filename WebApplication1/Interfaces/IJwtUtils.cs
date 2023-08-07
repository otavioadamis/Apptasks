using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IJwtUtils
    {
        public string CreateToken(User thisUser);
        public string? ValidateJwtToken(string token);
    }
}
