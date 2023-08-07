using Microsoft.AspNetCore.Mvc;
using WebApplication1.Authorization;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Models.DTOs;
using WebApplication1.Models.DTOs.ProjectTO_s;
using WebApplication1.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuthorize(Role.Admin)]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IJwtUtils _jwtUtils;
        private readonly IUserService _userService;
        private readonly ILogger<ProjectController> _logger;


        public ProjectController(IProjectService projectService, IJwtUtils jwtUtils, IUserService userService, ILogger<ProjectController> logger)
        {
            this._projectService = projectService;
            this._jwtUtils = jwtUtils;
            this._userService = userService;
            this._logger = logger;
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
            var createdProject = _projectService.CreateProject(user.Id, thisProject);
            return Ok(createdProject);
        }

        [CustomAuthorize(Role.Admin)]
        [HttpPatch("projects/{projectId}")]
        public ActionResult<Project> Update(string projectId, ProjectInfoDTO thisProject)
        {
            var user = HttpContext.Items["User"] as User;
            _projectService.UpdateProject(user.Id, projectId, thisProject);
            return Ok("Projeto atualizado!");
        }

        [CustomAuthorize(Role.Admin)]
        [HttpDelete("projects/{projectId}")]
        public ActionResult Delete(string projectId, ProjectDelDTO request)
        {
            var user = HttpContext.Items["User"] as User;
            var projectDeleted = _projectService.DeleteProject(user.Id, projectId, request);
            return Ok(projectDeleted);
        }

        [CustomAuthorize(Role.Admin)]
        [HttpPut("projects/{projectId}/cover")]
        public async Task<ActionResult> UploadImage(string projectId, IFormFile image)
        {
            var response = _projectService.UploadImage(projectId, image);
            return Ok(response);
        }

        [CustomAuthorize(Role.Admin)]
        [HttpGet("projects/{projectId}/cover")]
        public async Task<ActionResult> GetImage(string projectId)
        {
            var imageBytes = _projectService.GetImageFromProjectId(projectId);
            return File(imageBytes, "image/jpg");
        }

        [HttpGet("emails")]
        public async Task<ActionResult> GetEmails()
        {
            var emails = _projectService.GetEmails();
            return Ok(emails);  
        }
    }
}


