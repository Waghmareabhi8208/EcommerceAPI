using Ecommerce.API.DTOs;

namespace Ecommerce.API.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllAsync();

        Task<ProductResponseDto> AddAsync(ProductCreateDto dto);

        Task<ProductResponseDto?> GetByIdAsync(int id);

        Task<ProductResponseDto?> UpdateAsync(int id, ProductUpdateDto dto);
    }
}
