using MongoDB.Driver;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services
{
    public class ProjectService
    {
        private readonly IMongoCollection<Project> _projects;

        public ProjectService(IMongoCollection<Project> projects)
        {
            _projects = projects;
        }

        //CRUD

        //READ
        public List<Project> Get() => _projects.Find(project => true).ToList();
        public Project GetById(string id) => _projects.Find(project => project.Id == id).FirstOrDefault();

        //CREATE
        public Project Create(Project thisProject)
        {
            _projects.InsertOne(thisProject);
            return thisProject;
        }

        //UPDATE
        public void Update(string id, Project updatedProject) => _projects.ReplaceOne(project => project.Id == id, updatedProject);

        //DELETE
        public void Delete(string id) => _projects.DeleteOne(project => project.Id == id);

        //Methods

        public Project CreateProject(string _id, Project thisProject)
        {            

            var newProject = new Project()
            {
                Name = thisProject.Name,
                Description = thisProject.Description,
                CreatorId = _id              
            };

            Create(newProject);
            return newProject;
        }
    }
}
