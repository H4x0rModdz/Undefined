using Microsoft.EntityFrameworkCore;
using Undefined.Domain.Models;
using Undefined.Domain.Repositories;
using Undefined.Persistence.Contexts;

namespace Undefined.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<Product> AddProductAsync(Product product)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }
    }
}
