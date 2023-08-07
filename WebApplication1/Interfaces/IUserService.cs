using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Interfaces
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        UserResponseModel ChangePw(string _id, UserResetPwDTO thisUser);
        LoginResponseModel Signup(UserRegisterDTO thisUser);
        LoginResponseModel Login(UserLoginDTO thisUser);
        User UpdateInfo(string _id, UserUpdateDTO thisUser);
        string ForgotPassword(string email);
        User GetUserById(string _id);
        User GetUserByEmail(string email);
        User UpdateUser(string id, User updatedUser);
        UserResponseModel GetModel(User thisUser);
        void DeleteUser(string name);

    }
}