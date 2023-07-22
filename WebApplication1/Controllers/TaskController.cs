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

        [HttpGet("{projectName}/{taskName}")]
        [CustomAuthorize]
        public ActionResult<Task> GetTask(string projectName, string taskName)
        {
            var project = _projectService.GetByName(projectName);
                if (project == null) { return BadRequest("Not found!"); }

            var taskToFind = project.Tasks.Find(t => t.Name == taskName);
                if (taskToFind != null) 
            {
                return Ok(taskToFind);
            }
            return BadRequest("Cant find this task!");
        }

        [HttpPatch("addtask")]
        [CustomAuthorize(Role.Admin)]
        public ActionResult<Task> AddTask([FromQuery] string _id, Task thisTask)
        {
            var project = _projectService.GetById(_id);
                if(project == null) { return BadRequest("Not found!"); }

            if (project.Tasks == null)
            {
                project.Tasks = new List<Task>();
            }

            var createdTask = _taskService.CreateTask(thisTask);
            project.Tasks.Add(createdTask);
                _projectService.Update(_id, project);

            return Ok(createdTask);
        }

        [HttpPatch("assign")]
        [CustomAuthorize(Role.Admin)]
        public ActionResult<Task> AssignTask([FromQuery]string _id, AssignTaskModel request)
        {
            var project = _projectService.GetById(_id);
            if (project == null) { return BadRequest("Not found!"); }

            var user = _userService.GetByEmail(request.UserEmail);;
                if(user == null ) { return BadRequest("User not found!"); }
                else if (!project.Team.Exists(u => u.Equals(user.Id)))
                {
                    return BadRequest("Sorry! User is not on the team!");
                }
            
            var taskToAssign = project.Tasks.Find(t => t.Name == request.TaskName);
            if (taskToAssign != null)
            {
                taskToAssign.Responsable = user.Id;
                    _projectService.Update(project.Id, project);
                     return Ok("User assigned to task!");
            }

            return BadRequest("Error finding the task!");
        }
        
        //Todo, a better way to pass the project and taskname, because i can have two projects with the same name and task name.
        [CustomAuthorize(Role.Admin)]
        [HttpPatch("{projectName}/{taskName}")]
        public ActionResult<Task> UpdateTask(string projectName, string taskName, UpdateTaskDTO request) 
        {
            var project = _projectService.GetByName(projectName);
            if (project == null) { return BadRequest("Not found!"); }

            var taskToUpdate = project.Tasks.Find(t => t.Name == taskName);
            if (taskToUpdate != null) //Todo, create a service to update the task.
            {
                taskToUpdate.Name = request.Name;
                taskToUpdate.Description = request.Description;
                    return Ok("Task updated!");
            }
            return BadRequest("Error finding the task!");
        }


        [CustomAuthorize(Role.Admin)]
        [HttpDelete("{projectName}/{taskName}")]
        public ActionResult Delete(string projectName, string taskName)
        {
            var project = _projectService.GetByName(projectName);
                if (project == null) { return BadRequest("Not found!"); }

            var taskToRemove = project.Tasks.Find(t => t.Name == taskName);
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
