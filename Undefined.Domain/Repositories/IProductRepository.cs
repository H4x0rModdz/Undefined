using Undefined.Domain.Models;

namespace Undefined.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<Product> AddProductAsync(Product product);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
    }
}
