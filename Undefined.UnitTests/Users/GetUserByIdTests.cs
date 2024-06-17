using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Undefined.Domain.Models;
using Undefined.Domain.Repositories;
using Undefined.WebApi.Controllers;

namespace Undefined.UnitTests.Users
{
    public class GetUserByIdTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly UserController _controller;

        public GetUserByIdTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new UserController(_mockUserRepository.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ValidUser_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, UserName = "user" };
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            var result = await _controller.GetUserById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(userId, returnedUser.Id);
        }

        [Fact]
        public async Task InvalidUser_ReturnsNotFound()
        {
            var userId = It.IsAny<Guid>();
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _controller.GetUserById(userId);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
