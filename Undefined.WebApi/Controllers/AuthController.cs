using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Undefined.Domain.Models;
using Undefined.Domain.Models.Requests;
using Undefined.Domain.Repositories;
using Undefined.Persistence.Contexts;

namespace Undefined.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthController> logger, AppDbContext context)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login and get a JWT token")]
        [SwaggerResponse(200, "Login successful", typeof(string))]
        [SwaggerResponse(400, "Invalid username or password")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (request is null)
                return BadRequest();

            _logger.LogInformation("Login attempt for user {Username}", request.UserName);

            var user = await _userRepository.GetUserByUsernameAsync(request.UserName, request.Password);
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                _logger.LogWarning("Invalid login attempt for user {Username}", request.UserName);
                return BadRequest("Invalid username or password");
            }

            var token = GenerateJwtToken(user);

            _logger.LogInformation("Login successful for user {Username}", request.UserName);

            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register a new user")]
        [SwaggerResponse(201, "User successfully registered", typeof(User))]
        [SwaggerResponse(400, "Request is null or UserName already exists")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request is null)
            {
                _logger.LogWarning("Register request is null");
                return BadRequest();
            }

            _logger.LogInformation("Register attempt for user {Username}", request.UserName);

            if (UserNameExists(request.UserName))
            {
                _logger.LogWarning("User with username {Username} already exists", request.UserName);
                return BadRequest("UserName already exists");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                UserName = request.UserName,
                Password = hashedPassword,
            };

            try
            {
                await _context.AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User {Username} successfully registered with ID {UserId}", user.UserName, user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user {Username}", user.UserName);
                return StatusCode(500, "Internal server error");
            }

            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool UserNameExists(string userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }
    }
}
