using Ecommerce.API.Data;
using Ecommerce.API.DTOs;
using Ecommerce.API.Entities;
using Ecommerce.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        // Gives the list of All products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _service.GetAllAsync();

            return Ok(products);
        }

        [HttpPost]
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
    }
}
