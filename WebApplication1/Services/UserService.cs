using WebApplication1.Exceptions;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IJwtUtils _jwtUtils;

        public UserService(IUserRepository userRepository, IJwtUtils jwtUtils)
        {
            _userRepository = userRepository;
            _jwtUtils = jwtUtils;
        }

        //Get an User by Id (returns a model)
        public User GetUserById(string _id)
        {
            var user = _userRepository.GetById(_id);
            if (user == null) { throw new UserFriendlyException("User not found!"); }

            return user;
        }

        public User GetUserByEmail(string email)
        {
            var user = _userRepository.GetByEmail(email);
            if (user == null) { throw new UserFriendlyException("User not found!"); }
            return user;
        }

        public UserResponseModel GetModel(User thisUser)
        {
            var userModel = new UserResponseModel();
            userModel = userModel.CreateModel(thisUser);

            return userModel;
        }

        //Get all users (todo -> return only the model)
        public List<User> GetAllUsers()
        {
            var users = _userRepository.Get();
                if(users == null) { throw new UserFriendlyException("Theres nobody here!"); }
            return users;
        }

        public LoginResponseModel Signup(UserRegisterDTO thisUser)
        {
            var checkEmail = _userRepository.GetByEmail(thisUser.Email);
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

            _userRepository.Create(newUser);
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
            var user = _userRepository.GetByEmail(thisUser.Email);
            if (user == null)
            {
                throw new UserFriendlyException("Invalid credentials!");
            }

            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(thisUser.Password, user.Password);
            if (!isPasswordMatch) { throw new UserFriendlyException("Invalid credentials!"); }

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
            var user = _userRepository.GetByEmail(email);
            if (user == null) { throw new UserFriendlyException("User not found!"); }

            string token =
                _jwtUtils.CreateToken(user);

            return token;
        }

        public User UpdateUser(string id, User updatedUser)
        {
            var user = _userRepository.GetById(id);
                if (user == null) { throw new UserFriendlyException("User not found!"); }

            _userRepository.Update(id, updatedUser);
            var updatedU = _userRepository.GetById(id);
            return updatedU;
        }

        public User UpdateInfo(string _id, UserUpdateDTO thisUser)
        {
            var user = _userRepository.GetById(_id);
            if (user == null) { throw new UserFriendlyException("User not found!"); };

            _userRepository.Update(_id, thisUser);

            var updatedUser = _userRepository.GetById(_id);
            return updatedUser;
        }

        public UserResponseModel ChangePw(string _id, UserResetPwDTO thisUser)
        {

            var user = _userRepository.GetById(_id);
            if (user == null) { throw new UserFriendlyException("Cant find user!"); };

            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(thisUser.Password, user.Password);
            if (isPasswordMatch) { throw new UserFriendlyException("Password needs to be different than the current password"); }

            if (thisUser.Password != thisUser.ConfirmPassword) { throw new UserFriendlyException("Confirm Password and Passoword fiels must be equal"); }

            thisUser.Password = BCrypt.Net.BCrypt.HashPassword(thisUser.Password);

            _userRepository.UpdatePassword(_id, thisUser.Password);

            var updatedUser = _userRepository.GetById(_id);

            var userModel = new UserResponseModel();
            userModel = userModel.CreateModel(updatedUser);

            return userModel;

        }
        public void DeleteUser(string name)
        {
            var user = _userRepository.GetByName(name);
            if (user == null) { throw new UserFriendlyException("User not found!"); }

            _userRepository.Delete(user.Id);
        }

    }
    }
