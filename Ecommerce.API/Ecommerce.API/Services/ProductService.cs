using Ecommerce.API.DTOs;
using Ecommerce.API.Entities;
using Ecommerce.API.Interfaces;
using Microsoft.OpenApi.Validations;

namespace Ecommerce.API.Services
{
    public class ProductService : IProductService
    {   
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();

            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock
            }).ToList();
            
        }
        public async Task<ProductResponseDto> AddAsync(ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock
            };

            var createdProduct = await _repository.AddAsync(product);

            return new ProductResponseDto
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Description = createdProduct.Description,
                Price = createdProduct.Price,
                Stock = createdProduct.Stock
            };
        }

     
    }
}
