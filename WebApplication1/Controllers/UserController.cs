using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Xml.Linq;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public UserController(UserService userService, AuthService authService)
        {
            this._userService = userService;
            this._authService = authService;
        }

        [HttpGet()]
        public ActionResult<List<User>> Get() =>
            _userService.Get();

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

            string token = _authService.CreateToken(createdUser);

            var response = new LoginResponseModel
            {
                Token = token,
                Name = createdUser.Name
            };

            return Ok(response);

           //return CreatedAtRoute("GetUser", new { name = thisUser.Name.ToString() }, thisUser);
        }

        //LOGIN USER
        [HttpPost("login")]
        public ActionResult<LoginResponseModel> Login(UserLoginDTO thisUser)
        {

            var loggedUser = _userService.Login(thisUser);
            if (loggedUser == null) { return BadRequest("Invalid Password or Username"); }

            string token =
                _authService.CreateToken(loggedUser);

            var response = new LoginResponseModel
            {
                Token = token,
                Name = loggedUser.Name
            };


            return Ok(response);

        }

        //Update an User
        [HttpPut()]
        [Authorize]
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
                _authService.CreateToken(user);


            return Ok(token);
        }

        //Reset the user password, authorized with JwT Token
        [HttpPost("resetpassword")]
        [Authorize]
        public ActionResult<User> ResetPassword(UserResetPwDTO thisUser)
        {

            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value; //Get the user ID using jwt Token Claim

            var user = _userService.GetById(userId);

            if (user == null) { return BadRequest("Invalid Credentials"); }

            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(thisUser.Password, user.Password);
                if (isPasswordMatch) { return BadRequest("Password needs to be different than the current password"); }

            if(thisUser.Password != thisUser.ConfirmPassword) { return BadRequest("Confirm Password and Passoword fiels must be equal"); }

            _userService.ChangePw(userId, thisUser);
        
            return Ok("Password Changed!");
        }

        //UPDATE AN USER 
        /*       [HttpPut("{name}")]
               public ActionResult<User> Update(string name, User updatedUser) 
               {
                   var user = _userService.GetByName(name);

                   if (user == null) { return NotFound(); };

                   updatedUser.Id = user.Id;

                   _userService.Update(name, updatedUser);

                   return NoContent();

               }*/

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



 
