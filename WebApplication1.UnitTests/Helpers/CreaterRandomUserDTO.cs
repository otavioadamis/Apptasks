using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.UnitTests.Helpers
{
    public class CreaterRandomUserDTO
    {
        public UserResponseModel CreaterUser() 
        {
            return new()
            {
                Id = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Role = Role.User
            };
        }
    }
}
