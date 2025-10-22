using Microsoft.AspNetCore.Mvc;
using Store.G02.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IServiceManager _serviceManager) : ControllerBase
    {

        // priceasc
        // pricedesc
        // nameasc

        [HttpGet] // Get: baseUrl/api/products
        public async Task<IActionResult> GetAllProducts(int? brandId, int? typeId, string? sort, string? search)
        {
            var result = await _serviceManager.ProductService.GetAllProductsAsync(brandId, typeId, sort, search);
            if (result is null) return BadRequest(); // 400
            return Ok(result); // 200
        }

        [HttpGet("{id}")] // Get: baseUrl/api/products/5
        public async Task<IActionResult> GetProductById(int? id)
        {
            if(id is null)return BadRequest();

            var result = await _serviceManager.ProductService.GetProductByIdAsync(id.Value);

            if (result is null) return NotFound(); // 404

            return Ok(result); // 200
        }

        [HttpGet("brands")] // Get: baseUrl/api/brands
        public async Task<IActionResult> GetAllBrands()
        {
            var result = await _serviceManager.ProductService.GetAllBrandsAsync();
            if (result is null) return BadRequest(); // 400
            return Ok(result); // 200
        }

        [HttpGet("types")] // Get: baseUrl/api/types
        public async Task<IActionResult> GetAllTypes()
        {
            var result = await _serviceManager.ProductService.GetAllTypeAsync();
            if (result is null) return BadRequest(); // 400
            return Ok(result); // 200
        }
    }
}
