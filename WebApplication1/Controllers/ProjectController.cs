using Microsoft.AspNetCore.Mvc;
using WebApplication1.Authorization;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly JwtUtils _jwtUtils;
        private readonly UserService _userService;
       
        private readonly AuthService _authService;

        public ProjectController(ProjectService projectService, JwtUtils jwtUtils, AuthService authService, UserService userService)
        {
            this._projectService = projectService;
            this._jwtUtils = jwtUtils;
            this._userService = userService;
            
            this._authService = authService;
        }

        [HttpGet("projects")]
        public ActionResult<List<Project>> Get()
        {
            return _projectService.Get();
        }


        [HttpPost("create")]
        [CustomAuthorize]
        public ActionResult<Project> Create(Project thisProject) 
        {
            var user = HttpContext.Items["User"] as User;
            if (user == null) { return BadRequest("An error ocurred, try again."); }

            var createdProject = _projectService.CreateProject(user.Id, thisProject);


           return Ok(createdProject);

        }
    }
}
