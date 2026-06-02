using Ecommerce.API.DTOs;
using Ecommerce.API.DTOs.Common;
using Ecommerce.API.Entities;
using Ecommerce.API.Interfaces;
using Microsoft.OpenApi.Validations;

namespace Ecommerce.API.Services
{
    public class ProductService : IProductService
    {   
        private readonly IProductRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public ProductService(IProductRepository repository, IWebHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
        }

        public async Task<List<ProductResponseDto>> 
            GetAllAsync(
                ProductQueryParams queryParams)
        {
            var products = await _repository.GetAllAsync(queryParams);

            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl
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
                Stock = createdProduct.Stock,
                ImageUrl = createdProduct.ImageUrl
            };
        }

        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if(product == null)
                return null;

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl
            };
        }

        public async Task<ProductResponseDto?> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock
            };

            var updateProduct = await _repository.UpdateAsync(id, product);

            if(updateProduct == null)
                return null;

            return new ProductResponseDto
            {
                Id = updateProduct.Id,
                Name = updateProduct.Name,
                Description = updateProduct.Description,
                Price = updateProduct.Price,
                Stock = updateProduct.Stock,
                ImageUrl = updateProduct.ImageUrl
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<string?> UploadImageAsync(int productId, IFormFile file)
        {
            var product = await _repository.GetByIdAsync(productId);

            if (product == null)
            {
                return null;
            }

            // Create Images folder if not exists
            string imagesFolder =
                Path.Combine(
                    _environment.WebRootPath,
                    "images");

            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            // Delete old image if exists
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                string oldImagePath =
                    Path.Combine(
                        _environment.WebRootPath,
                        product.ImageUrl.TrimStart('/'));

                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            // Generate unique file name
            string fileName =
                $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            string filePath =
                Path.Combine(imagesFolder, fileName);

            // Save file
            using (var stream =
                new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save image path in database
            product.ImageUrl = $"/images/{fileName}";

            await _repository.UpdateAsync(productId, product);

            return product.ImageUrl;
        }
    }
}
