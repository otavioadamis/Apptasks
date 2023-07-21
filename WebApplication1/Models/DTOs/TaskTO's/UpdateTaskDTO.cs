using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebApplication1.Models.DTOs.TaskTO_s
{
    public class UpdateTaskDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
