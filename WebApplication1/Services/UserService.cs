using Amazon.Util.Internal.PlatformServices;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services
{


    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoCollection<User> users)
        {
            _users = users;
        }
        
        //Todo: create generic repositories to simple generic CRUD operations, so i avoid duplicating code.
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
        public void Update(string name, User updatedUser) => _users.ReplaceOne(user => user.Name == name, updatedUser);

        public void Update(string _id, UserUpdateDTO thisUser)
        {
            var updateEmail = Builders<User>.Update.Set(o => o.Email, thisUser.Email);
            _users.UpdateOne(o => o.Id == _id , updateEmail);

            var updateName = Builders<User>.Update.Set(o => o.Name, thisUser.Name);
            _users.UpdateOne(o => o.Id == _id, updateName);
        }
        //DELETE
        public void Delete(string name) => _users.DeleteOne(user => user.Name == name);
                      
        public User Signup(UserRegisterDTO thisUser)
        {
           
            thisUser.Password = BCrypt.Net.BCrypt.HashPassword(thisUser.Password);

            var newUser = new User()
            {
                Email = thisUser.Email,
                Name = thisUser.Name,
                Password = thisUser.Password,
                Role = thisUser.Role
            };
            
            Create(newUser);
            return newUser;
        }
       
        public User Login(UserLoginDTO thisUser)
        {
           
           var user = GetByEmail(thisUser.Email);                      
            if(user == null)
            {
                return null;
            }

            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(thisUser.Password, user.Password);
             if(!isPasswordMatch) { return null; }

            return user;          
        }

        public User UpdateInfo(string _id, UserUpdateDTO thisUser) 
        {
            var user = GetById(_id);
                if (user == null) { return null; };

            Update(_id, thisUser);

            var updatedUser = GetById(_id);
            return updatedUser;

        }

        public User ChangePw(string _id, UserResetPwDTO thisUser)
        {
            var user = GetById(_id);
            if (user == null) { return null; };

            thisUser.Password = BCrypt.Net.BCrypt.HashPassword(thisUser.Password);

            var updatePw = Builders<User>.Update.Set(o => o.Password, thisUser.Password);
            _users.UpdateOne(o => o.Id == _id, updatePw);

            var updatedUser = GetById(_id);
            return updatedUser;

        }
    }
}
