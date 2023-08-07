using Amazon.Runtime.Internal;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WebApplication1.Exceptions;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.Models.DTOs.TaskTO_s;
using Task = WebApplication1.Models.Task;

namespace WebApplication1.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;

        public TaskService(IUserService userService, IProjectService projectService)
        {
            _userService = userService;
            _projectService = projectService;
        }

        public Task GetTask(string projectId, string taskId)
        {
            var project = _projectService.GetById(projectId);
            if (project == null) { throw new UserFriendlyException("Project not found!"); }

            var taskToFind = project.Tasks.Find(t => t.Id == taskId);
            if (taskToFind != null)
            {
                return taskToFind;
            }
            throw new UserFriendlyException("Cant find this task!");
        }

        public Task CreateTask(string projectId, Task thisTask)
        {

            var project = _projectService.GetById(projectId);
            if (project == null) { throw new UserFriendlyException("Project not found!"); }

            if (project.Tasks == null)
            {
                project.Tasks = new List<Task>();
            }

            var newTask = new Task()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = thisTask.Name,
                Description = thisTask.Description,
                IsCompleted = false,
                DeadLine = thisTask.DeadLine
            };

            project.Tasks.Add(newTask);
            _projectService.Update(projectId, project);

            return newTask;
        }

        public Task AssignTask(string projectId, string taskId, AssignTaskModel request)
        {
            var project = _projectService.GetById(projectId);
            if (project == null) { throw new UserFriendlyException("Not found!"); }

            var user = _userService.GetUserByEmail(request.UserEmail);
            if (user == null) { throw new UserFriendlyException("User not found!"); }

            else if (!project.Team.Exists(u => u.Equals(user.Id)))
            {
                throw new UserFriendlyException("Sorry! User is not on the team!");
            }

            var taskToAssign = project.Tasks.Find(t => t.Id == taskId);
            
            if (taskToAssign != null)
            {
                if (user.tasksId == null) { user.tasksId = new List<string>(); }
                else if (user.tasksId.Contains(taskToAssign.Id)) { throw new UserFriendlyException("User is already assigned to this task!"); }      
                
                user.tasksId.Add(taskToAssign.Id);
                taskToAssign.Responsable = user.Id;

                _userService.UpdateUser(user.Id, user);
                _projectService.Update(project.Id, project);
                
                return taskToAssign;
            }
            throw new UserFriendlyException("Error finding the task!");
        }

        public Task Update(string projectId, string taskId, UpdateTaskDTO request)
        {
            var project = _projectService.GetById(projectId);
            if (project == null) { throw new UserFriendlyException("Not found!"); }

            var taskToUpdate = project.Tasks.Find(t => t.Id == taskId);
            if (taskToUpdate != null)
            {
                taskToUpdate.Name = request.Name;
                taskToUpdate.Description = request.Description;
                return taskToUpdate;
            }
            throw new UserFriendlyException("Error finding the task!");
        }

        public Task IsCompleted(string projectId, string taskId, TaskMarkDTO request, string userId)
        {
            var project = _projectService.GetById(projectId);
            if (project == null) { throw new UserFriendlyException("Not found!"); }

            var task = project.Tasks.Find(t => t.Id == taskId);
            if (task.Responsable != userId) { throw new UserFriendlyException("Sorry! Only the responsable of the task can mark as completed or not!"); }
            if (task != null)
            {
                task.IsCompleted = request.IsCompleted;
                _projectService.Update(projectId, project);
                return task;
            }
            throw new UserFriendlyException("Error finding the task!");
        }

        public Project Delete(string projectId, string taskId)
        {
            var project = _projectService.GetById(projectId);
            if (project == null) { throw new UserFriendlyException("Not found!"); }

            var taskToRemove = project.Tasks.Find(t => t.Id == taskId);
            if (taskToRemove != null)
            {
                project.Tasks.Remove(taskToRemove);
                _projectService.Update(project.Id, project);
                return project;
            }
            throw new UserFriendlyException("Error finding the task!");
        }
    }
}
