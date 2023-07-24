﻿using WebApplication1.Authorization;
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
            if (user == null) { return NotFound("User not found!"); }

            return user;
        }

        //REGISTER NEW USER
        [HttpPost()]
        public ActionResult<LoginResponseModel> Signup(UserRegisterDTO thisUser)
        {
            try
            {
                var createdUser = _userService.Signup(thisUser);              
                return Ok(createdUser);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        //LOGIN USER
        [HttpPost("login")]
        public ActionResult<LoginResponseModel> Login(UserLoginDTO thisUser)
        {
            try
            {
                var loggedUser = _userService.Login(thisUser);
                return Ok(loggedUser);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        //Update an User
        [CustomAuthorize]
        [HttpPatch("{userId}")]
        public ActionResult<User> UpdateInfo(string userId, UserUpdateDTO thisUser)
        {
            try 
            {
                var updatedUser = _userService.UpdateInfo(userId, thisUser);
                return Ok(updatedUser);
            }
            catch (Exception ex) { return  BadRequest(ex.Message); }
        }

        //Forgot Password, send email (with jwt token) to the user
        [HttpPost()]
        [Route("forgotpassword")]
        public ActionResult<string> ForgotPassword([FromBody] string thisEmail)
        {
            try
            {
                var tokenEmail = _userService.ForgotPassword(thisEmail);
                return Ok(tokenEmail);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        //Reset the user password, authorized with JwT Token
        //Todo, put the logic in services
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

        [CustomAuthorize(Role.Admin)]
        [HttpDelete("{name}")]
        public ActionResult<User> Delete(string name)
        {
            try
            {
                _userService.DeleteUser(name);
                return Ok("User has been deleted!");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}




 
