using FluentAssertions;
using Moq;
using WebApplication1.Exceptions;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;
using WebApplication1.UnitTests.Fixtures;
using Task = System.Threading.Tasks.Task;

namespace WebApplication1.UnitTests.Systems.Services
{
    public class TestUserService
    {
        

        [Fact]
        public async Task GetUserById_WhenValidId_ShouldReturnUser()
        {
            // Arrange
            var userRepositoryStub = new Mock<IUserRepository>();
            var jwtUtilsStub = new Mock<IJwtUtils>();

            var sut = new UserService(userRepositoryStub.Object, jwtUtilsStub.Object);

            // Act
            var testUser = UsersFixture.GetOneTestUser();
            userRepositoryStub
                .Setup(repo => repo.GetById(It.IsAny<string>()))
                .Returns(testUser);

            var result = sut.GetUserById(It.IsAny<string>());

            // Assert
            result.Should().BeOfType<User>();
        }

        [Fact]
        public async Task GetUserById_WhenInvalidId_ShouldReturnUserNotFound()
        {
            // Arrange
            var userRepositoryStub = new Mock<IUserRepository>();
            var jwtUtilsStub = new Mock<IJwtUtils>();

            var sut = new UserService(userRepositoryStub.Object, jwtUtilsStub.Object);

            // Act
            userRepositoryStub
                .Setup(repo => repo.GetById(It.IsAny<string>()))
                .Returns((User)null);

            // Assert
            Assert.Throws<UserFriendlyException>(() => sut.GetUserById(It.IsAny<string>()));
        }

        [Fact]
        public async Task GetAllUsers_WhenCalled_ShouldReturnListOfUser()
        {
            // Arrange
            var userRepositoryStub = new Mock<IUserRepository>();
            var jwtUtilsStub = new Mock<IJwtUtils>();

            var sut = new UserService(userRepositoryStub.Object, jwtUtilsStub.Object);

            // Act
            var testUsers = UsersFixture.GetTestUsers();
            userRepositoryStub
                .Setup(repo => repo.Get())
                .Returns(testUsers);

            var result = sut.GetAllUsers();

            // Assert
            result.Should().BeOfType<List<User>>();
        }

        [Fact]
        public void SignupNewUser_WithValidEmail_ReturnsLoginResponseModel()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDTO
            {
                Email = "newuser@example.com",
                Name = "New User",
                Password = "password123",
                Role = Role.User
            };

            var userRepositoryStub = new Mock<IUserRepository>();
            userRepositoryStub
                .Setup(repo => repo.GetByEmail(userRegisterDto.Email))
                .Returns((User)null); // Simulating that the email is not already used

            var jwtUtilsStub = new Mock<IJwtUtils>();
            jwtUtilsStub
                .Setup(utils => utils.CreateToken(It.IsAny<User>()))
                .Returns("mocked-token"); // Mocking JWT token creation

            var sut = new UserService(userRepositoryStub.Object, jwtUtilsStub.Object);

            // Act
            var result = sut.Signup(userRegisterDto);

            // Assert
            result.Should().BeOfType<LoginResponseModel>();
            result.Token.Should().Be("mocked-token"); // Make sure the token matches the mocked value
            result.user.Should().BeEquivalentTo(userRegisterDto, options => options.ExcludingMissingMembers());
        }

        [Fact]
        public void SignupNewUser_WithInvalidEmail_ReturnsEmailAlreadyUsed()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDTO
            {
                Email = "newuser@example.com",
                Name = "New User",
                Password = "password123",
                Role = Role.User
            };

            var jwtUtilsStub = new Mock<IJwtUtils>();
            
            var userRepositoryStub = new Mock<IUserRepository>();
            userRepositoryStub
                .Setup(repo => repo.GetByEmail(userRegisterDto.Email))
                .Returns(UsersFixture.GetOneTestUser()); // Simulating that the email is already used

            var sut = new UserService(userRepositoryStub.Object, jwtUtilsStub.Object);

            // Assert
            Assert.Throws<UserFriendlyException>(() => sut.Signup(userRegisterDto));
        }
    }
}
