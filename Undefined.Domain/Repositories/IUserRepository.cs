using Undefined.Domain.Models;

namespace Undefined.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByUsernameAsync(string username, string password);
        Task<User> UpdateUserAsync(Guid id, User user);
    }
}
