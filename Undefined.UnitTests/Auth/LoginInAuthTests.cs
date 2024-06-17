using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Undefined.Domain.Models;
using Undefined.Domain.Models.Requests;
using Undefined.Domain.Repositories;
using Undefined.Persistence.Contexts;
using Undefined.WebApi.Controllers;

namespace Undefined.UnitTests.Auth
{
    public class LoginInAuthTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _authController;
        private readonly AppDbContext _context;

        public LoginInAuthTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _authController = new AuthController(_userRepositoryMock.Object, _configurationMock.Object, _loggerMock.Object, _context);

            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("9Zs1P#u$K!mL!q1ZpNc1Cz5UeX0ApLoP");
        }

        [Fact]
        public async Task ValidLoginRequest_ReturnsOk()
        {
            var loginRequest = new LoginRequest
            {
                UserName = "testuser",
                Password = "testpass"
            };

            _userRepositoryMock.Setup(repo =>
                                repo.GetUserByUsernameAsync(loginRequest.UserName,
                                    BCrypt.Net.BCrypt.HashPassword(loginRequest.Password))).ReturnsAsync(new User
                                    {
                                        Id = It.IsAny<Guid>(),
                                        UserName = "testuser",
                                        Password = "testpass"
                                    });

            var result = await _authController.Login(loginRequest);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task InvalidLoginRequest_ReturnsBadRequest()
        {
            var result = await _authController.Login(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task InvalidLoginCredentials_ReturnsBadRequest()
        {
            var loginRequest = new LoginRequest();

            _userRepositoryMock.Setup(repo =>
                    repo.GetUserByUsernameAsync(loginRequest.UserName, loginRequest.Password))
                .ReturnsAsync((User)null);

            var result = await _authController.Login(loginRequest);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid username or password", badRequestResult.Value);
        }
    }
}
