using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            this._userService = userService;
        }

        [HttpGet()]
        public ActionResult<List<User>> Get() =>
            _userService.Get();

        [HttpGet("{name}", Name = "GetUser")]
        public ActionResult<User> GetByName(string name)
        {
            var user = _userService.GetByName(name);

            if(user == null) { return NotFound(); }

            return user;
        }

        
        //REGISTER NEW USER
        [HttpPost("register")]
        public ActionResult<User> Signup(User thisUser)
        {        
            _userService.Signup(thisUser);
            return CreatedAtRoute("GetUser", new { name = thisUser.Name.ToString() }, thisUser);
        }

        //LOGIN USER
        [HttpPost("login")]
        public ActionResult<User> Login(User thisUser)
        {

            var loggedUser = _userService.Login(thisUser);
            if(loggedUser == null) { return BadRequest("Invalid Password or Username"); }
            return Ok(loggedUser);

        }

        //UPDATE AN USER 
        [HttpPut("{name}")]
        public ActionResult<User> Update(string name, User updatedUser) 
        {
            var user = _userService.GetByName(name);

            

            if (user == null) { return NotFound(); };

            updatedUser.Id = user.Id;

            _userService.Update(name, updatedUser);

            return NoContent();

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




        /*[HttpGet("register")]
        public IActionResult RegisterUser([FromQuery] string username, [FromQuery] string password, [FromQuery] string email) //O FromQuery indica que o elemento deve vir de um Query no url
        {
            User newUser = userRegister.RegisterUser(username, password, email);

            if (newUser == null)
            {
                return BadRequest("deu ruim");
            }
            Console.WriteLine("deu bom requisitei");
            
            return Ok(newUser);
        }*/
    }
}
 
