using Account.Core.Dtos;
using Account.Core.Dtos.Program;
using Account.Core.Models;
using Account.Core.Services.Programe;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Account.Apis.Controllers
{

    public class CategoriesController : ApiBaseController
    {
        private readonly ICategoryRepository _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryRepository categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// Retrieves all categories with pagination.
        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] PaginationParameters paginationParameters)
        {
            try
            {
                var pagedCategories = await _categoryService.GetAllCategoriesAsync(paginationParameters);
                return Ok(new { Message = "Categories retrieved successfully.", Data = pagedCategories });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve categories.");
                return StatusCode(500, new { Message = "An error occurred while retrieving categories." });
            }
        }


        /// Retrieves a specific category by its ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return Ok(new { Message = "Category retrieved successfully.", Data = category });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Category with ID {id} not found.");
                return NotFound(new { Message = "Category not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve category with ID {id}.");
                return StatusCode(500, new { Message = "An error occurred while retrieving the category." });
            }
        }

        /// Creates a new category.
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest(new { Message = "Invalid category data." });
            }

            try
            {
                var createdCategory = await _categoryService.CreateCategoryAsync(categoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, new { Message = "Category created successfully.", Data = createdCategory });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create category.");
                return StatusCode(500, new { Message = "An error occurred while creating the category." });
            }
        }

        /// Updates an existing category.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest(new { Message = "Invalid category data." });
            }

            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryDto);
                return Ok(new { Message = "Category updated successfully.", Data = updatedCategory });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Category with ID {id} not found.");
                return NotFound(new { Message = "Category not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update category with ID {id}.");
                return StatusCode(500, new { Message = "An error occurred while updating the category." });
            }
        }

        // DELETE: api/categories/delete-multiple
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleCategories([FromForm] IEnumerable<int> ids)
        {
            try
            {
                var deletedCount = await _categoryService.DeleteMultipleCategoriesAsync(ids);

                if (deletedCount == 0)
                    return NotFound(new ContentContainer<string>(null, "No matching categories found to delete."));

                return Ok(new ContentContainer<int>(deletedCount, $"{deletedCount} categories deleted successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting multiple categories.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while deleting the categories."));
            }
        }

    }
}
