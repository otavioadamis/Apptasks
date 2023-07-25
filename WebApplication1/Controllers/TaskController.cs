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

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly JwtUtils _jwtUtils;
        private readonly UserService _userService;
        private readonly TaskService _taskService;

        private readonly AuthService _authService;

        public TaskController(ProjectService projectService, JwtUtils jwtUtils, AuthService authService, UserService userService, TaskService taskService)
        {
            this._projectService = projectService;
            this._jwtUtils = jwtUtils;
            this._userService = userService;
            this._taskService = taskService;
            
            this._authService = authService;

        }

        [HttpGet("projects/{projectId}/tasks/{taskId}")]
        [CustomAuthorize]
        public ActionResult<Task> GetTask(string projectId, string taskId)
        {
            try
            {
                var task = _taskService.GetTask(projectId, taskId);
                return task;
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("projects/{projectId}")]
        [CustomAuthorize(Role.Admin)]
        public ActionResult<Task> AddTask(string projectId, Task thisTask)
        {
            try
            {
                var createdTask = _taskService.CreateTask(projectId, thisTask);
                return Ok(createdTask);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPatch("projects/{projectId}/tasks/{taskId}/assign")]
        [CustomAuthorize(Role.Admin)]
        public ActionResult<Task> AssignTask(string projectId, string taskId, AssignTaskModel request)
        {
            try
            {
                var assignedTask = _taskService.AssignTask(projectId, taskId, request);
                return Ok(assignedTask);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
        
        [CustomAuthorize(Role.Admin)]
        [HttpPatch("projects/{projectId}/tasks/{taskId}")]
        public ActionResult<Task> UpdateTask(string projectId, string taskId, UpdateTaskDTO request) 
        {
            try
            {
                var updatedTask = _taskService.Update(projectId, taskId, request);
                return Ok(updatedTask);
            }
            catch(Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPatch("projects/{projectId}/tasks/{taskId}/markcomplete")]
        [CustomAuthorize]
        public ActionResult<Task> IsCompleted(string projectId, string taskId, TaskMarkDTO request)
        {
            var user = HttpContext.Items["User"] as User;
            try
            {
                var taskIsCompleted = _taskService.IsCompleted(projectId, taskId, request, user.Id);
                return Ok(taskIsCompleted);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [CustomAuthorize(Role.Admin)]
        [HttpDelete("projects/{projectId}/tasks/{taskId}")]
        public ActionResult Delete(string projectId, string taskId)
        {
            try
            {
                var deleteTask = _taskService.Delete(projectId, taskId);
                return Ok(deleteTask);           
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
  }
