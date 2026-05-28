using Ecommerce.API.Data;
using Ecommerce.API.DTOs;
using Ecommerce.API.DTOs.Common;
using Ecommerce.API.Entities;
using Ecommerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        // Gives the list of All products
        [HttpGet]
        public async Task<IActionResult> 
            GetProducts(
                [FromQuery]
                ProductQueryParams queryParams)
        {
            var products = await _service.GetAllAsync(queryParams);

            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddProduct(ProductCreateDto dto)
        {
            var product = await _service.AddAsync(dto);

            return Ok(product);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _service.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product); 
        }

        [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateProduct(int id,ProductUpdateDto dto)
        {
            var product = await _service.UpdateAsync(id,dto);

            if(product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if(!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
