using Ecommerce.API.Entities;

namespace Ecommerce.API.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();

        Task<Product> AddAsync(Product product);

        Task<Product?> GetByIdAsync(int  id);

        Task<Product?> UpdateAsync(int id,Product product);

        Task<bool> DeleteAsync(int id);
    }
}
