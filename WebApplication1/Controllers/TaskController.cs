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
            var project = _projectService.GetById(projectId);
                if (project == null) { return BadRequest("Not found!"); }

            var taskToFind = project.Tasks.Find(t => t.Id == taskId);
                if (taskToFind != null) 
            {
                return Ok(taskToFind);
            }
            return BadRequest("Cant find this task!");
        }

        [HttpPut("projects/{projectId}")]
        [CustomAuthorize(Role.Admin)]
        public ActionResult<Task> AddTask(string projectId, Task thisTask)
        {
            var project = _projectService.GetById(projectId);
                if(project == null) { return BadRequest("Not found!"); }

            if (project.Tasks == null)
            {
                project.Tasks = new List<Task>();
            }

            var createdTask = _taskService.CreateTask(thisTask);
            project.Tasks.Add(createdTask);
                _projectService.Update(projectId, project);

            return Ok(createdTask);
        }

        [HttpPatch("projects/{projectId}/tasks/{taskId}/assign")]
        [CustomAuthorize(Role.Admin)]
        public ActionResult<Task> AssignTask(string projectId, string taskId, AssignTaskModel request)
        {
            var project = _projectService.GetById(projectId);
            if (project == null) { return BadRequest("Not found!"); }

            var user = _userService.GetByEmail(request.UserEmail);;
                if(user == null ) { return BadRequest("User not found!"); }
                else if (!project.Team.Exists(u => u.Equals(user.Id)))
                {
                    return BadRequest("Sorry! User is not on the team!");
                }
            
            var taskToAssign = project.Tasks.Find(t => t.Id == taskId);
            if (taskToAssign != null)
            {
                taskToAssign.Responsable = user.Id;
                    _projectService.Update(project.Id, project);
                     return Ok("User assigned to task!");
            }
            return BadRequest("Error finding the task!");
        }
        
        [CustomAuthorize(Role.Admin)]
        [HttpPatch("projects/{projectId}/tasks/{taskId}")]
        public ActionResult<Task> UpdateTask(string projectId, string taskId, UpdateTaskDTO request) 
        {
            var project = _projectService.GetById(projectId);
            if (project == null) { return BadRequest("Not found!"); }

            var taskToUpdate = project.Tasks.Find(t => t.Id == taskId);
            if (taskToUpdate != null)
            {
                taskToUpdate.Name = request.Name;
                taskToUpdate.Description = request.Description;
                    return Ok("Task updated!");
            }
            return BadRequest("Error finding the task!");
        }

        [HttpPatch("projects/{projectId}/tasks/{taskId}/markcomplete")]
        [CustomAuthorize]
        public ActionResult<Task> IsCompleted(string projectId, string taskId, TaskMarkDTO request)
        {
            var project = _projectService.GetById(projectId);
            if (project == null) { return BadRequest("Not found!"); }
            
            var task = project.Tasks.Find(t => t.Id == taskId);
            if (task != null)
            {
                task.IsCompleted = request.IsCompleted;
                _projectService.Update(projectId, project);
                return Ok(project);
            }
            return BadRequest("Error finding the task!");
        }

        [CustomAuthorize(Role.Admin)]
        [HttpDelete("projects/{projectId}/tasks/{taskId}")]
        public ActionResult Delete(string projectId, string taskId)
        {
            var project = _projectService.GetById(projectId);
                if (project == null) { return BadRequest("Not found!"); }

            var taskToRemove = project.Tasks.Find(t => t.Id == taskId);
                if(taskToRemove != null)
            {
                project.Tasks.Remove(taskToRemove);
                _projectService.Update(project.Id, project);
                    return Ok("Task deleted!");
            }
            return BadRequest("Error finding the task!");
        }
    }
  }
