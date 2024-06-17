using Microsoft.EntityFrameworkCore;
using Undefined.Domain.Models;

namespace Undefined.Persistence.Contexts
{
    public static class Seeders
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            SeedUsers(modelBuilder);
            SeedProducts(modelBuilder);
        }

        private static void SeedUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData([
                new User { Id = Guid.NewGuid(), UserName = "admin", Password = BCrypt.Net.BCrypt.HashPassword("admin") },
                new User { Id = Guid.NewGuid(), UserName = "user", Password = BCrypt.Net.BCrypt.HashPassword("user") },
            ]);
        }

        private static void SeedProducts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData([
                new Product { Id = 1, Name = "Product 1"},
                new Product { Id = 2, Name = "Product 2"},
            ]);
        }
    }
}
