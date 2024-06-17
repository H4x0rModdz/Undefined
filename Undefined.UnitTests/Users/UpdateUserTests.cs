using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Undefined.Domain.Models;
using Undefined.Domain.Repositories;
using Undefined.WebApi.Controllers;

namespace Undefined.UnitTests.Users
{
    public class UpdateUserTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<UserController>> _loggerMock;
        private readonly UserController _userController;

        public UpdateUserTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserController>>();

            _userController = new UserController(_userRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ValidUpdateRequest_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, UserName = "testuser", Password = "testpass" };

            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(userId, user))
                               .ReturnsAsync(user);

            var result = await _userController.UpdateUser(userId, user);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task InvalidUserData_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = Guid.NewGuid(), UserName = "testuser", Password = "testpass" };

            var result = await _userController.UpdateUser(userId, user);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UserNotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, UserName = "testuser", Password = "testpass" };

            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(userId, user))
                               .ReturnsAsync((User)null);

            var result = await _userController.UpdateUser(userId, user);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
