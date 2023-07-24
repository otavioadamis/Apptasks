namespace WebApplication1.Models.DTOs
{
    public class LoginResponseModel
    {
        public string Token { get; set; }
        public UserResponseModel user { get; set; }

    }
}
