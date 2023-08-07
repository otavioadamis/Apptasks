using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WebApplication1.Authorization;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.Models.DTOs.TaskTO_s;
using WebApplication1.Services;
using Task = WebApplication1.Models.Task;
using WebApplication1.Helpers;
using WebApplication1.Interfaces;
using WebApplication1.Services.EmailServices;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IJwtUtils _jwtUtils;
        private readonly IUserService _userService;
        private readonly ITaskService _taskService;
        private readonly EmailService _emailService;

        private readonly ILogger<TaskController> _logger;

        public TaskController(IProjectService projectService, IJwtUtils jwtUtils, IUserService userService, ITaskService taskService, EmailService emailService, ILogger<TaskController> logger)
        {
            this._projectService = projectService;
            this._jwtUtils = jwtUtils;
            this._userService = userService;
            this._taskService = taskService;
            this._logger = logger;
            this._emailService = emailService;
        }

        [HttpGet("projects/{projectId}/tasks/{taskId}")]
        [CustomAuthorize]
        public ActionResult<Task> GetTask(string projectId, string taskId)
        {
            var task = _taskService.GetTask(projectId, taskId);
            return task;
        }

        [HttpPut("projects/{projectId}")]
        [CustomAuthorize(Role.Admin)]
        public ActionResult<Task> AddTask(string projectId, Task thisTask)
        {
            var createdTask = _taskService.CreateTask(projectId, thisTask);
            return Ok(createdTask);
        }

        [HttpPatch("projects/{projectId}/tasks/{taskId}/assign")]
        [CustomAuthorize(Role.Admin)]
        public ActionResult<Task> AssignTask(string projectId, string taskId, AssignTaskModel request)
        {
            var assignedTask = _taskService.AssignTask(projectId, taskId, request);
            return Ok(assignedTask);
        }

        [CustomAuthorize(Role.Admin)]
        [HttpPatch("projects/{projectId}/tasks/{taskId}")]
        public ActionResult<Task> UpdateTask(string projectId, string taskId, UpdateTaskDTO request)
        {
            var updatedTask = _taskService.Update(projectId, taskId, request);
            return Ok(updatedTask);
        }

        [HttpPatch("projects/{projectId}/tasks/{taskId}/markcomplete")]
        [CustomAuthorize]
        public ActionResult<Task> IsCompleted(string projectId, string taskId, TaskMarkDTO request)
        {
            var user = HttpContext.Items["User"] as User;
            var taskIsCompleted = _taskService.IsCompleted(projectId, taskId, request, user.Id);
            return Ok(taskIsCompleted);
        }

        [CustomAuthorize(Role.Admin)]
        [HttpDelete("projects/{projectId}/tasks/{taskId}")]
        public ActionResult Delete(string projectId, string taskId)
        {
            var deleteTask = _taskService.Delete(projectId, taskId);
            return Ok(deleteTask);
        }

        [HttpGet("sendemail")]
        public async Task<ActionResult> SendEmail()
        {
            _emailService.SendEmailAsync(_projectService.GetEmails());
            return Ok("emails sended!");
        }
    }
  }
