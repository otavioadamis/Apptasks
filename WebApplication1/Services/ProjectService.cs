using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Security.Cryptography;
using WebApplication1.Exceptions;
using WebApplication1.Extensions;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.Models.DTOs.ProjectTO_s;
using Task = WebApplication1.Models.Task;

namespace WebApplication1.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IMongoCollection<Project> _projects;
        private readonly IUserService _userService;

        public ProjectService(IMongoCollection<Project> projects, IUserService userService)
        {
            _projects = projects;
            _userService = userService;
        }
        //CRUD

        //READ
        public List<Project> Get() => _projects.Find(project => true).ToList();
        public Project GetById(string id) => _projects.Find(project => project.Id == id).FirstOrDefault();
        public Project GetByName(string name) => _projects.Find(project => project.Name == name).FirstOrDefault();
        //CREATE
        public Project Create(Project thisProject)
        {
            _projects.InsertOne(thisProject);
            return thisProject;
        }
        //UPDATE
        public void Update(string id, Project updatedProject) => _projects.ReplaceOne(project => project.Id == id, updatedProject);

        public void Update(string _id, ProjectInfoDTO thisProject)
        {
            var updateName = Builders<Project>.Update.Set(o => o.Name, thisProject.Name);
            _projects.UpdateOne(o => o.Id == _id, updateName);

            var updateDescription = Builders<Project>.Update.Set(o => o.Description, thisProject.Description);
            _projects.UpdateOne(o => o.Id == _id, updateDescription);

        }
        //DELETE
        public void Delete(string id) => _projects.DeleteOne(project => project.Id == id);

        //Methods

        public Project GetProjectById(string projectId)
        {
            var project = GetById(projectId);
            if (project == null) { throw new UserFriendlyException("Project not finded!"); }

            return project;
        }

        public Project CreateProject(string _id, Project thisProject)
        {

            var creator = _userService.GetUserById(_id);
            if (creator == null) { throw new UserFriendlyException("An error ocurred!"); }

            var newProject = new Project()
            {
                Name = thisProject.Name,
                Description = thisProject.Description,
                CreatorId = _id,
            };

            newProject.Team = new List<string>();
            foreach (string email in thisProject.Team)
            {
                var user = _userService.GetUserByEmail(email);
                    if(user == null) { throw new UserFriendlyException("This email: " + email + " does not exist in our system!"); }
                newProject.Team.Add(user.Id);
            }

            Create(newProject);
            return newProject;
        }

        public Project UpdateProject(string userId, string projectId, ProjectInfoDTO thisProject)
        {
            var project = GetById(projectId);
            if (project == null) { throw new UserFriendlyException("Project not found!"); }

            else if (userId != project.CreatorId)
            {
                throw new UserFriendlyException("Sorry! You are not the owner of this project!");
            }

            Update(projectId, thisProject);
            var updatedProject = GetById(projectId);

            return updatedProject;
        }

        public Project DeleteProject(string userId, string projectId, ProjectDelDTO request)
        {
            var user = _userService.GetUserById(userId);

            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isPasswordMatch) { throw new UserFriendlyException("Wrong password"); }

            var project = GetById(projectId);
            if (project == null) { throw new UserFriendlyException("Cant find this project!"); }

            Delete(project.Id); return project;
        }

        public Project UploadImage(string projectId, IFormFile image)
        {
            var project = GetById(projectId);
            if (project == null) { throw new UserFriendlyException("Cant find this project!"); }

            if (image != null)
            {
                using (var stream = image.OpenReadStream())
                {
                    var image_data = stream.ToByteArray();
                    project.Cover = image_data;
                }
                Update(projectId, project);
                var updatedProject = GetById(projectId);

                return updatedProject;
            }
            throw new UserFriendlyException("Error while uploading the image!");
        }

        public Byte[] GetImageFromProjectId(string projectId)
        {
            var project = GetById(projectId);
            if (project == null) { throw new UserFriendlyException("Project not found!"); }

            var imageBytes = project.Cover;
            if (imageBytes != null) { return imageBytes; }

            throw new UserFriendlyException("Image not found!");
        }

        //should return list of emails
        public HashSet<string> GetEmails()
        {
            HashSet<string> emails = new HashSet<string>();

            var dateNow = DateTime.Today;         
            var projects = Get();
            foreach (var project in projects)
            {
                var tasksFromProject = project.Tasks;
                if(tasksFromProject == null) { continue; }
                foreach(var task in tasksFromProject)
                {
                    if(task.IsCompleted == true) { continue; }
                    
                    var deadline = task.DeadLine;
                    var difference = new DateTime(deadline.Value.Year, deadline.Value.Month, deadline.Value.Day) - dateNow;
                    
                    if(difference.Days == 1)
                    {
                        var _id = task.Responsable;
                        if(_id == null) { continue; }
                        
                        var user = _userService.GetUserById(_id);
                        emails.Add(user.Email);
                    }
                }
            }
            return emails;
        }
    }
}
