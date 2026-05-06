using Ecommerce.API.Entities;

namespace Ecommerce.API.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();

        Task<Product> AddAsync(Product product);
    }
}
