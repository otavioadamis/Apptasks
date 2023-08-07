using WebApplication1.Models;
using WebApplication1.Models.DTOs.ProjectTO_s;

namespace WebApplication1.Interfaces
{
    public interface IProjectService
    {
        Project Create(Project thisProject);
        Project CreateProject(string _id, Project thisProject);
        void Delete(string id);
        Project DeleteProject(string userId, string projectId, ProjectDelDTO request);
        List<Project> Get();
        Project GetById(string id);
        Project GetByName(string name);
        Project GetProjectById(string projectId);
        void Update(string id, Project updatedProject);
        void Update(string _id, ProjectInfoDTO thisProject);
        Project UpdateProject(string userId, string projectId, ProjectInfoDTO thisProject);
        Project UploadImage(string projectId, IFormFile image);
        Byte[] GetImageFromProjectId(string projectId);
        HashSet<string> GetEmails();
    }
}