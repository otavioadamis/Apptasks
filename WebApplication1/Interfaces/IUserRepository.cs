using MongoDB.Driver;
using WebApplication1.Models.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IUserRepository
    {
        List<User> Get();
        User GetByName(string name);
        User GetByEmail(string email);
        User GetById(string id);
        User Create(User thisUser);
        void Update(string id, User updatedUser);
        void Update(string id, UserUpdateDTO thisUser);
        void Delete(string id);
        void UpdatePassword(string id, string newPassword);
    }
}