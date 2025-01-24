using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Models.Entites;
using ShopSystem.Core.Models;
using ShopSystem.Core.Services.Programe;

namespace Shop_System.Controllers
{
    [Authorize]
    public class ProductsController : ApiBaseController
    {
        private readonly IProductRepository _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        #region Get All Products 
        //// GET: api/products
        //[HttpGet]
        //public async Task<IActionResult> GetAllProducts([FromQuery] PaginationParameters paginationParameters)
        //{
        //    var products = await _productService.GetAllProductsAsync(paginationParameters);
        //    return Ok(products);
        //} 
        #endregion

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            var products = await _productService.GetAllProductsAsync(paginationParameters, queryOptions);
            return Ok(new { Message = "Products retrieved successfully.", Data = products });
        }


        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            return Ok(product);
        }

        // POST: api/products/AddProducts
        [HttpPost("AddProducts")]
        public async Task<IActionResult> CreateProducts([FromBody] IEnumerable<ProductDTO> productDtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdProducts = await _productService.CreateProductsAsync(productDtos);
                return CreatedAtAction(nameof(GetAllProducts), new { count = createdProducts.Count },
                    new ContentContainer<List<ProductDTO>>(createdProducts, $"{createdProducts.Count} products added successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ContentContainer<string>(null, ex.Message));
            }
            catch (InvalidOperationException ex) // Handle duplicate UniqueNumber
            {
                _logger.LogWarning(ex.Message);
                return Conflict(new ContentContainer<string>(null, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating products.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while processing your request."));
            }
        }




        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedProduct = await _productService.UpdateProductAsync(id, productDto);
            if (updatedProduct == null)
                return NotFound("Product not found for update.");

            return Ok(updatedProduct);
        }

        // DELETE: api/products/delete-multiple
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleProducts([FromForm] IEnumerable<int> ids)
        {
            try
            {
                var deletedCount = await _productService.DeleteMultipleProductsAsync(ids);

                if (deletedCount == 0)
                    return NotFound(new ContentContainer<string>(null, "No matching products found to delete."));

                return Ok(new ContentContainer<int>(deletedCount, $"{deletedCount} products deleted successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting multiple products.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while processing your request."));
            }
        }



        // PATCH: api/products/{productId}/stock
        [HttpPatch("{productId}/stock")]
        public async Task<IActionResult> UpdateProductStock(int productId, [FromQuery] int quantityChange)
        {
            try
            {
                await _productService.UpdateProductStockAsync(productId, quantityChange);
                return Ok(new { Message = "Product stock updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { Message = "Product not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating stock for product ID {productId}.");
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }



        // GET: api/products/category/{categoryId}
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        // GET: api/products/{productId}/stock
        [HttpGet("{productId}/stock")]
        public async Task<IActionResult> GetAvailableStock(int productId)
        {
            var stock = await _productService.GetAvailableStockAsync(productId);

            if (stock == null)
            {
                return NotFound(new ContentContainer<int?>(null, $"Product with ID {productId} not found."));
            }

            return Ok(new ContentContainer<int?>(stock, "Stock retrieved successfully."));
        }
    }
}
