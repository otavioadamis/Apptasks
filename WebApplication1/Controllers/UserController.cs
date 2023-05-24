using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRegister userRegister;

        public UserController(UserRegister userRegister)
        {
            this.userRegister = userRegister;
        }

        [HttpGet("register")]
        public IActionResult RegisterUser(string username, string password, string email)
        {
            User newUser = userRegister.RegisterUser(username, password, email);

            if (newUser != null)
            {
                return BadRequest("deu ruim");
            }
            Console.WriteLine("deu bom requisitei");
            return Ok(newUser);
        }
    }
}
