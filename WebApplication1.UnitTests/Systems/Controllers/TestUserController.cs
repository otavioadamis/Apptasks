using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Exceptions;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.UnitTests.Fixtures;

namespace WebApplication1.UnitTests.Systems.Controllers
{
    public class TestUserController
    {
        [Fact]
        public void Get_OnSuccess_InvokeUserServiceGet_ExactlyOnce()
        {
            // Arrenge
            var userServiceStub = new Mock<IUserService>();
            var jwtUtilsStub = new Mock<IJwtUtils>();
            var loggerStub = new Mock<ILogger<UserController>>();

            userServiceStub
                .Setup(service => service.GetAllUsers())
                .Returns(UsersFixture.GetTestUsers());

            var sut = new UserController(userServiceStub.Object, jwtUtilsStub.Object, loggerStub.Object);
            // Act
            var result = sut.Get();

            // Assert
            userServiceStub
                .Verify(service => service.GetAllUsers(),
                Times.Once()
              );
        }

        [Fact]
        public void Get_OnSuccess_ReturnsListOfUsers()
        {
            // Arrange
            var userServiceStub = new Mock<IUserService>();
            var jwtUtilsStub = new Mock<IJwtUtils>();
            var loggerStub = new Mock<ILogger<UserController>>();

            userServiceStub
                .Setup(service => service.GetAllUsers())
                .Returns(UsersFixture.GetTestUsers());

            var sut = new UserController(userServiceStub.Object, jwtUtilsStub.Object, loggerStub.Object);
            // Act
            var result = sut.Get();

            // Assert
            result.Should().BeOfType<ActionResult<List<User>>>();

        }

        [Fact]
        public void Get_OnSuccess_ReturnsStatusCode200()
        {
            // Arrange
            var userServiceStub = new Mock<IUserService>();
            var jwtUtilsStub = new Mock<IJwtUtils>();
            var loggerStub = new Mock<ILogger<UserController>>();

            userServiceStub
                .Setup(service => service.GetAllUsers())
                .Returns(UsersFixture.GetTestUsers());

            var sut = new UserController(userServiceStub.Object, jwtUtilsStub.Object, loggerStub.Object);

            // Act
            var result = sut.Get();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        //Todo -> does this make any sense?
        [Fact]
        public void Get_WithNonUsers_ReturnsStatusCode400()
        {
            // Arrange
            var userServiceStub = new Mock<IUserService>();
            var jwtUtilsStub = new Mock<IJwtUtils>();
            var loggerStub = new Mock<ILogger<UserController>>();

            userServiceStub
                .Setup(service => service.GetAllUsers())
                .Throws(new UserFriendlyException("a"));

            var sut = new UserController(userServiceStub.Object, jwtUtilsStub.Object, loggerStub.Object);

            // Act
            // Assert
            Assert.Throws<UserFriendlyException>(() => sut.Get());
        }

        [Fact]
        public void GetUserById_WithExistingId_ReturnsExpectedUserResponseModel()
        {
            // Arrange
            var expectedUser = UsersFixture.GetOneTestUser();

            var userServiceStub = new Mock<IUserService>();
            var jwtUtilsStub = new Mock<IJwtUtils>();
            var loggerStub = new Mock<ILogger<UserController>>();

            userServiceStub
                .Setup(service => service.GetUserById(expectedUser.Id))
                .Returns(expectedUser);

            userServiceStub
                .Setup(service => service.GetModel(expectedUser))
                .Returns(new UserResponseModel());

            var sut = new UserController(userServiceStub.Object, jwtUtilsStub.Object, loggerStub.Object);

            // Act
            var result = sut.GetById(expectedUser.Id);

            // Assert
            result.Should().BeOfType<ActionResult<UserResponseModel>>();
        }
    }
}