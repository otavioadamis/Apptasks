using Microsoft.AspNetCore.Mvc;
using WebApplication1.Authorization;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Models.DTOs;
using WebApplication1.Models.DTOs.ProjectTO_s;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuthorize(Role.Admin)]
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

        [HttpPost()]
        public ActionResult<Project> Create(Project thisProject)
        {
            var user = HttpContext.Items["User"] as User;
            if (user == null) { return BadRequest("An error ocurred, try again."); }

            var createdProject = _projectService.CreateProject(user.Id, thisProject);

            return Ok(createdProject);
        }

        [CustomAuthorize(Role.Admin)]
        [HttpPatch()]
        public ActionResult<Project> Update(string _id, ProjectInfoDTO thisProject)
        {
            var user = HttpContext.Items["User"] as User;

            var project = _projectService.GetById(_id);
            if (project == null) { return BadRequest("Project not found!"); }
            else if (user.Id != project.CreatorId)
            {
                return BadRequest("Sorry! You are not the owner of this project!");
            }

            _projectService.UpdateProject(_id, thisProject);
            return Ok("Projeto atualizado!");
        }

        [CustomAuthorize(Role.Admin)]
        [HttpDelete()]
        public ActionResult Delete(ProjectDelDTO request)
        {
            var user = HttpContext.Items["User"] as User;

            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!isPasswordMatch) { return BadRequest("Wrong password"); }

            var project = _projectService.GetByName(request.ProjectName);
            if (project == null) { return BadRequest("Cant find this project!"); }

            _projectService.Delete(project.Id);
            return Ok("Project deleted!");
        }

        [CustomAuthorize]
        [HttpGet("{name}")]
        public ActionResult<Project> Get(string name) 
        {
            var project = _projectService.GetByName(name);
            if (project == null) { return BadRequest("Project not finded!"); }

            return Ok(project);                
        }
    }
}
