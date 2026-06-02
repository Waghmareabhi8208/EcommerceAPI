using Ecommerce.API.DTOs;
using Ecommerce.API.DTOs.Common;

namespace Ecommerce.API.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllAsync(ProductQueryParams queryParams);

        Task<ProductResponseDto> AddAsync(ProductCreateDto dto);

        Task<ProductResponseDto?> GetByIdAsync(int id);

        Task<ProductResponseDto?> UpdateAsync(int id, ProductUpdateDto dto);

        Task<bool> DeleteAsync(int id);
        Task<string> UploadImageAsync(int productId,IFormFile file);
        Task<bool> DeleteImageAsync(int productId);
    }
}
