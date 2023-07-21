﻿using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Cryptography;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.Models.DTOs.TaskTO_s;
using Task = WebApplication1.Models.Task;

namespace WebApplication1.Services
{
    public class TaskService
    {
        private readonly UserService _userService;
        private readonly ProjectService _projectService;

        public TaskService(UserService userService, ProjectService projectService)
        {
            this._userService = userService;
            this._projectService = projectService;
        }
        public Task CreateTask(Task thisTask)
        {
            var newTask = new Task()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = thisTask.Name,
                Description = thisTask.Description,
                IsCompleted = false
            };               
            return newTask;
        }
    }
}