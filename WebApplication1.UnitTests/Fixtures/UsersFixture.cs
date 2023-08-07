using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.UnitTests.Fixtures
{
    public static class UsersFixture
    {
        public static User GetOneTestUser() => new()
        {
            Id = "3",
            Email = "test3@email.com",
            Name = "TestUser3",
            Password = "passwor3",
            Role = Role.User
        };

        public static List<User> GetTestUsers() => new()
        {
            new User
            {
                Id = "1",
                Email = "test1@email.com",
                Name = "TestUser1",
                Password = "password",
                Role = Role.User
            },
            new User
            {
                Id = "2",
                Email = "test2@email.com",
                Name = "TestUser2",
                Password = "password2",
                Role = Role.Admin
            }
        };
    }
}
