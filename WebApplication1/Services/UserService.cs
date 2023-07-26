using Amazon.Util.Internal.PlatformServices;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using WebApplication1.Authorization;
using WebApplication1.Controllers;
using WebApplication1.Exceptions;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services
{


    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly JwtUtils _jwtUtils;

        public UserService(IMongoCollection<User> users, JwtUtils jwtUtils)
        {
            _users = users;
            _jwtUtils = jwtUtils;
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
        public void Update(string name, User updatedUser) => _users.ReplaceOne(user => user.Name == name, updatedUser);

        public void Update(string _id, UserUpdateDTO thisUser)
        {
            var updateEmail = Builders<User>.Update.Set(o => o.Email, thisUser.Email);
            _users.UpdateOne(o => o.Id == _id , updateEmail);

            var updateName = Builders<User>.Update.Set(o => o.Name, thisUser.Name);
            _users.UpdateOne(o => o.Id == _id, updateName);
        }
        //DELETE
        public void Delete(string id) => _users.DeleteOne(user => user.Id == id);
         
        //Get an User by Id (sending a model)
        public UserResponseModel GetUserById(string _id)
        {
            var user = GetById(_id);
                if(user == null) { throw new UserFriendlyException("User not found!"); }

            var userModel = new UserResponseModel();
            userModel = userModel.CreateModel(user);
            return userModel;
        }

        public LoginResponseModel Signup(UserRegisterDTO thisUser)
        {
            var checkEmail = GetByEmail(thisUser.Email);
            if (checkEmail != null)
            {
                throw new UserFriendlyException("Sorry! This email has already been used!");
            }
            thisUser.Password = BCrypt.Net.BCrypt.HashPassword(thisUser.Password);

            var newUser = new User()
            {
                Email = thisUser.Email,
                Name = thisUser.Name,
                Password = thisUser.Password,
                Role = thisUser.Role
            }; 
            
            Create(newUser);
            string token = _jwtUtils.CreateToken(newUser);

            var userModel = new UserResponseModel();
            userModel = userModel.CreateModel(newUser);

            var res = new LoginResponseModel
            {
                Token = token,
                user = userModel
            };

            return res;
        }
       
        public LoginResponseModel Login(UserLoginDTO thisUser)
        {        
           var user = GetByEmail(thisUser.Email);                      
            if(user == null)
            {
                throw new UserFriendlyException("Invalid credentials!");
            }

            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(thisUser.Password, user.Password);
             if(!isPasswordMatch) { throw new UserFriendlyException("Invalid credentials!"); }

            string token = _jwtUtils.CreateToken(user);

            var userModel = new UserResponseModel();
            userModel = userModel.CreateModel(user);

            var res = new LoginResponseModel
            {
                Token = token,
                user = userModel
            };
            return res;
        }

        public string ForgotPassword(string email)
        {
            var user = GetByEmail(email);
            if (user == null) { throw new UserFriendlyException("User not found!"); }
            
            string token =
                _jwtUtils.CreateToken(user);

            return token;
        }

        public User UpdateInfo(string _id, UserUpdateDTO thisUser) 
        {
            var user = GetById(_id);
                if (user == null) { throw new UserFriendlyException("User not found!"); };

            Update(_id, thisUser);

            var updatedUser = GetById(_id);
            return updatedUser;

        }

        public UserResponseModel ChangePw(string _id, UserResetPwDTO thisUser)
        {

            var user = GetById(_id);
                if (user == null) { throw new UserFriendlyException("Cant find user!"); };

            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(thisUser.Password, user.Password);
            if (isPasswordMatch) { throw new UserFriendlyException("Password needs to be different than the current password"); }

            if (thisUser.Password != thisUser.ConfirmPassword) { throw new UserFriendlyException("Confirm Password and Passoword fiels must be equal"); }
    
            thisUser.Password = BCrypt.Net.BCrypt.HashPassword(thisUser.Password);

            var updatePw = Builders<User>.Update.Set(o => o.Password, thisUser.Password);
            _users.UpdateOne(o => o.Id == _id, updatePw);

            var updatedUser = GetById(_id);
            
            var userModel = new UserResponseModel();
                userModel = userModel.CreateModel(updatedUser);
            
            return userModel;

        }
        public void DeleteUser(string name)
        {
            var user = GetByName(name);
            if (user == null) { throw new UserFriendlyException("User not found!"); }

            Delete(user.Id);
        }
    }
}
