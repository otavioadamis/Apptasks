using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class UserRegister
    {

        public User RegisterUser(string username, string password, string email)
        {
            User newUser = new User
            {
                username = username,
                password = password,
                email = email
            };

            return newUser;
        }
    }
}
