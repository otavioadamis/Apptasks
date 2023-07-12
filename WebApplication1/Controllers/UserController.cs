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

        [CustomAuthorize(Role.Admin)]
        [HttpGet()]
        public ActionResult<List<User>> Get()
        {
            return _userService.Get();
        }

        [HttpGet("{name}", Name = "GetUser")]
        public ActionResult<User> GetByName(string name)
        {
            var user = _userService.GetByName(name);

            if (user == null) { return NotFound(); }

            return user;
        }

        //REGISTER NEW USER
        [HttpPost()]
        public ActionResult<User> Signup(UserRegisterDTO thisUser)
        {
            var checkEmail = _userService.GetByEmail(thisUser.Email);
            if (checkEmail != null) //existe um usuario com esse email ou nome
            {
                return BadRequest("Email ja cadastrado");
            }

            var createdUser =
                _userService.Signup(thisUser);

            string token = _jwtUtils.CreateToken(createdUser);

            var response = new LoginResponseModel
            {
                Token = token,
                Name = createdUser.Name
            };

            return Ok(response);

        }

        //LOGIN USER
        [HttpPost("login")]
        public ActionResult<LoginResponseModel> Login(UserLoginDTO thisUser)
        {

            var loggedUser = _userService.Login(thisUser);
            if (loggedUser == null) { return BadRequest("Invalid Password or Username"); }

            string token =
                _jwtUtils.CreateToken(loggedUser);

            var response = new LoginResponseModel
            {
                Token = token,
                Name = loggedUser.Name
            };

            return Ok(response);

        }

        //Update an User
        [CustomAuthorize]
        [HttpPut()]
        public ActionResult<User> UpdateInfo([FromQuery] string _id, UserUpdateDTO thisUser)
        {
            var updatedUser = 
                _userService.UpdateInfo(_id, thisUser);

            if(updatedUser == null) { return NotFound(); }

            return Ok(updatedUser);
        }

        //Forgot Password, send email (with jwt token) to the user
        [HttpPost()]
        [Route("forgotpassword")]
        public ActionResult<User> ForgotPassword([FromBody] string thisEmail)
        {
            var user = _userService.GetByEmail(thisEmail);

            if (user == null) { return BadRequest("User not found"); }

            string token =
                _jwtUtils.CreateToken(user);


            return Ok(token);
        }

        //Reset the user password, authorized with JwT Token
        [HttpPost("resetpassword")]
        [CustomAuthorize]
        public ActionResult<User> ResetPassword(UserResetPwDTO thisUser)
        {

            var user = HttpContext.Items["User"] as User;

            if (user == null) { return BadRequest("Invalid Credentials"); }

            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(thisUser.Password, user.Password);
                if (isPasswordMatch) { return BadRequest("Password needs to be different than the current password"); }

            if(thisUser.Password != thisUser.ConfirmPassword) { return BadRequest("Confirm Password and Passoword fiels must be equal"); }

            _userService.ChangePw(user.Id, thisUser);
        
            return Ok("Password Changed!");
        }

        //DELETE AN USER
        
        [HttpDelete("{name}")]
        public ActionResult<User> Delete(string name)
        {
            var user = _userService.GetByName(name);

            if (user == null) { return NotFound(); }

            _userService.Delete(user.Name);
            
            return NoContent();
        }
    }
}




 
