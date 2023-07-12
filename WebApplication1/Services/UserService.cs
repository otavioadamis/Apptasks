using Amazon.Util.Internal.PlatformServices;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public UserService(IUsersDatabaseSettings settings)
        {
            var client = new MongoClient();
            var database = client.GetDatabase();

            _users = database.GetCollection<User>();
        }

        //METODOS PADROES

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

        //DELETE
        public void Delete(string name) => _users.DeleteOne(user => user.Name == name);

               
        //MEUS METODOS REGISTRATION E AUTH
            
        //Signup

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
            var oldUser = GetById(_id);
                if (oldUser == null) { return null; };

            var newUser = new User()
            {
                Id = oldUser.Id,
                Email = thisUser.Email,
                Name = thisUser.Name,
                Password = oldUser.Password
            };

            Update(oldUser.Name , newUser);

            return newUser;

        }

        public User ChangePw(string _id, UserResetPwDTO thisUser)
        {
            var oldUser = GetById(_id);
            if (oldUser == null) { return null; };

            thisUser.Password = BCrypt.Net.BCrypt.HashPassword(thisUser.Password);

            var newUser = new User()
            {
                Id = oldUser.Id,
                Email = oldUser.Email,
                Name = oldUser.Name,

                Password = thisUser.Password
            };

                        Update(oldUser.Name, newUser);

            return newUser;
        }
    }
}
