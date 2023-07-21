using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPatch("addtask")]
        [CustomAuthorize(Role.Admin)]
        public ActionResult<Task> AddTask([FromQuery] string _id, Models.Task thisTask)
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
                else if (!project.Team.Exists(x => x.Equals(user.Id)))
                {
                    return BadRequest("Sorry! User is not on the team!");
                } 

                //Todo: make this a function in Task Services
            foreach (Task task in project.Tasks)
            {
                Console.WriteLine(task.Name);
                if (task.Name == request.TaskName)
                {
                    task.Responsable = user.Id;
                        _projectService.Update(_id, project);
                    return Ok();
                }
            }
            return BadRequest("Error finding the task!");
        }

        [CustomAuthorize(Role.Admin)]
        [HttpPatch("{projectName}/{taskName}/update")]
        public ActionResult<Task> UpdateTask(string projectName, string taskName, UpdateTaskDTO thisTask) 
        {
            var project = _projectService.GetByName(projectName);
            if (project == null) { return BadRequest("Not found!"); }

            //Todo: make this a function in Task Services
            foreach (Task task in project.Tasks)
            {
                if (task.Name == taskName)
                {
                    task.Name = thisTask.Name;
                    task.Description = thisTask.Description;

                        _projectService.Update(project.Id, project);
                    return Ok("Task updated!");
                }
            }
            return BadRequest("Error finding the task!");
        }
    }
  }
