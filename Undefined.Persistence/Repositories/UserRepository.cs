using Microsoft.EntityFrameworkCore;
using Undefined.Domain.Models;
using Undefined.Domain.Repositories;
using Undefined.Persistence.Contexts;

namespace Undefined.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);

        }

        public async Task<User> GetUserByUsernameAsync(string username, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User> UpdateUserAsync(Guid id, User user)
        {
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser != null)
                {
                    existingUser.UserName = user.UserName;
                    existingUser.Password = user.Password;

                    await _context.SaveChangesAsync();
                }
                return existingUser;
            }
        }
    }
}
