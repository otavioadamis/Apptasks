using MongoDB.Driver;
using WebApplication1.Interfaces;
using WebApplication1.Models.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoCollection<User> users)
        {
            _users = users;         
        }

        //CRUD

        //READ
        public List<User> Get() => _users.Find(user => true).ToList();
        public User GetByName(string name) => _users.Find(user => user.Name == name).FirstOrDefault();
        public User GetByEmail(string email) => _users.Find(user => user.Email == email).FirstOrDefault();
        public User GetById(string id) => _users.Find(user => user.Id == id).FirstOrDefault();
        //CREATE
        public User Create(User thisUser)
        {
            _users.InsertOne(thisUser);
            return thisUser;
        }

        //UPDATE
        public void Update(string _id, User updatedUser) => _users.ReplaceOne(user => user.Id == _id, updatedUser);

        public void Update(string _id, UserUpdateDTO thisUser)
        {
            var updateEmail = Builders<User>.Update.Set(o => o.Email, thisUser.Email);
            _users.UpdateOne(o => o.Id == _id, updateEmail);

            var updateName = Builders<User>.Update.Set(o => o.Name, thisUser.Name);
            _users.UpdateOne(o => o.Id == _id, updateName);
        }

        //DELETE
        public void Delete(string id) => _users.DeleteOne(user => user.Id == id);

        public void UpdatePassword(string id, string newPassword)
        {
            var updatePw = Builders<User>.Update.Set(o => o.Password, newPassword);
            _users.UpdateOne(o => o.Id == id, updatePw);
        }
    }
}
