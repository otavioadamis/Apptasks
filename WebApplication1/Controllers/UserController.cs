using WebApplication1.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Xml.Linq;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;
using Amazon.Runtime;
using WebApplication1.Helpers;
using Microsoft.Net.Http.Headers;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtUtils _jwtUtils;
        private readonly AuthService _authService;

        public UserController(UserService userService, JwtUtils jwtUtils, AuthService authService)
        {
            this._userService = userService;
            this._jwtUtils = jwtUtils;
            this._authService = authService;
        }

        //Todo, update this method to send just the ResponseModels from all users. 
        [CustomAuthorize(Role.Admin)]
        [HttpGet()]
        public ActionResult<List<User>> Get()
        {
            return _userService.Get();
        }

        [HttpGet("{id}")]
        public ActionResult<UserResponseModel> GetById(string id)
        {
            var userModel = _userService.GetUserById(id);
            return userModel;     
        }

        //REGISTER NEW USER
        [HttpPost()]
        public ActionResult<LoginResponseModel> Signup(UserRegisterDTO thisUser)
        {                     
            var createdUser = _userService.Signup(thisUser);              
            return Ok(createdUser);           
        }

        //LOGIN USER
        [HttpPost("login")]
        public ActionResult<LoginResponseModel> Login(UserLoginDTO thisUser)
        {
            var loggedUser = _userService.Login(thisUser);
            return Ok(loggedUser);           
        }

        //Update an User
        [CustomAuthorize]
        [HttpPatch("{userId}")]
        public ActionResult<User> UpdateInfo(string userId, UserUpdateDTO thisUser)
        {
            var updatedUser = _userService.UpdateInfo(userId, thisUser);
            return Ok(updatedUser);
        }

        //Forgot Password, send email (with jwt token) to the user
        [HttpPost()]
        [Route("forgotpassword")]
        public ActionResult<string> ForgotPassword([FromBody] string thisEmail)
        {

            var tokenEmail = _userService.ForgotPassword(thisEmail);
            return Ok(tokenEmail);

        }

        //Reset the user password, authorized with JwT Token
        //Todo, put the logic in services
        [HttpPost("resetpassword")]
        [CustomAuthorize]
        public ActionResult<UserResponseModel> ResetPassword(UserResetPwDTO thisUser)
        {
        var user = HttpContext.Items["User"] as User;
            if (user == null) { return BadRequest("Invalid Credentials"); }

        var userNewPw = _userService.ChangePw(user.Id, thisUser);
            return Ok(userNewPw);
        }

        //DELETE AN USER

        [CustomAuthorize(Role.Admin)]
        [HttpDelete("{name}")]
        public ActionResult<User> Delete(string name)
        {
            _userService.DeleteUser(name);
            return Ok("User has been deleted!");
        }
    }
}




 
