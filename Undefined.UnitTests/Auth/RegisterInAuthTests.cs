using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class RegisterInAuthTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _authController;
        private readonly AppDbContext _context;

        public RegisterInAuthTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;
            _context = new AppDbContext(options);

            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            
            _authController = new AuthController(_userRepositoryMock.Object, _configurationMock.Object, _loggerMock.Object, _context);
        }

        [Fact]
        public async Task ValidRequest_ReturnsCreatedAtAction()
        {
            var request = new RegisterRequest
            {
                UserName = "testuser",
                Password = "testpass"
            };

            var result = await _authController.Register(request);

            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, ((CreatedAtActionResult)result).StatusCode);
        }

        [Fact]
        public async Task InvalidRequest_ReturnsBadRequest()
        {
            var result = await _authController.Register(null);

            Assert.IsType<BadRequestResult>(result);
        }
    }
}
