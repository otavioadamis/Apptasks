using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.Models.DTOs.TaskTO_s;

namespace WebApplication1.Interfaces
{
    public interface ITaskService
    {
        Models.Task AssignTask(string projectId, string taskId, AssignTaskModel request);
        Models.Task CreateTask(string projectId, Models.Task thisTask);
        Project Delete(string projectId, string taskId);
        Models.Task GetTask(string projectId, string taskId);
        Models.Task IsCompleted(string projectId, string taskId, TaskMarkDTO request, string userId);
        Models.Task Update(string projectId, string taskId, UpdateTaskDTO request);
    }
}