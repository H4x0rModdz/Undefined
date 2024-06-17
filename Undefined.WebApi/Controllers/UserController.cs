using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Undefined.Domain.Models;
using Undefined.Domain.Repositories;

namespace Undefined.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public UserController(IUserRepository userRepository, ILogger logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
        }

        [Authorize] 
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an existing user")]
        [SwaggerResponse(200, "User successfully updated", typeof(User))]
        [SwaggerResponse(400, "Invalid user data or ID mismatch")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "User not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User user)
        {
            _logger.LogInformation("Received request to update user. UserId: {Id}", id);
            if (user is null || id != user.Id)
            {
                _logger.LogWarning("Invalid user data or ID mismatch. UserId: {Id}", id);
                return BadRequest();
            }

            var updatedUser = await _userRepository.UpdateUserAsync(id, user);
            if (updatedUser is null)
            {
                _logger.LogWarning("User not found. UserId: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("User successfully updated. UserId: {Id}", id);
            return Ok(updatedUser);
        }

        [Authorize]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a user by ID")]
        [SwaggerResponse(200, "User found", typeof(User))]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            _logger.LogInformation("Received request to get user by ID. UserId: {Id}", id);
            
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user is null)
            {
                _logger.LogWarning("User not found. UserId: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("User found. UserId: {Id}", id);
            return Ok(user);
        }
    }
}
