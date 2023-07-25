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

        [HttpGet()]
        public ActionResult<List<Project>> Get()
        {
            return _projectService.Get();
        }

        [CustomAuthorize]
        [HttpGet("projects/{projectId}")]
        public ActionResult<Project> Get(string projectId)
        {
            var project = _projectService.GetProjectById(projectId);
            return Ok(project);
        }

        [CustomAuthorize(Role.Admin)]
        [HttpPost()]
        public ActionResult<Project> Create(Project thisProject)
        {
            var user = HttpContext.Items["User"] as User;
            try
            {
                var createdProject = _projectService.CreateProject(user.Id, thisProject);
                return Ok(createdProject);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [CustomAuthorize(Role.Admin)]
        [HttpPatch("projects/{projectId}")]
        public ActionResult<Project> Update(string projectId, ProjectInfoDTO thisProject)
        {
            var user = HttpContext.Items["User"] as User;
            try
            {
                _projectService.UpdateProject(user.Id, projectId, thisProject);
                return Ok("Projeto atualizado!");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [CustomAuthorize(Role.Admin)]
        [HttpDelete("projects/{projectId}")]
        public ActionResult Delete(string projectId ,ProjectDelDTO request)
        {
            var user = HttpContext.Items["User"] as User;
            try
            {
                var projectDeleted = _projectService.DeleteProject(user.Id, projectId, request);
                return Ok(projectDeleted);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
