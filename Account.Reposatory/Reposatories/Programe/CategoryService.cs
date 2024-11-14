using Account.Core.Dtos;
using Account.Core.Dtos.Program;
using Account.Core.Models.Entites;
using Account.Core.Services.Programe;
using Account.Reposatory.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Reposatory.Reposatories.Programe
{
    public class CategoryService : ICategoryRepository
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(StoreContext context, IMapper mapper, ILogger<CategoryService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<CategoryDTO>> GetAllCategoriesAsync(PaginationParameters paginationParameters)
        {
            try
            {
                var query = _context.Categories.AsQueryable(); // Assuming Categories is your DbSet<Category>

                // Apply pagination
                var totalItems = await query.CountAsync();
                var categories = await query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                var categoryDtos = _mapper.Map<List<CategoryDTO>>(categories); // Map entities to DTOs

                return new PagedResult<CategoryDTO>
                {
                    Items = categoryDtos,
                    TotalCount = totalItems,
                    PageNumber = paginationParameters.PageNumber,
                    PageSize = paginationParameters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all categories.");
                throw; // Rethrow the exception to be handled by the caller
            }
        }


        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDto)
        {
            var categoryEntity = _mapper.Map<Category>(categoryDto);
            await _context.Categories.AddAsync(categoryEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<CategoryDTO>(categoryEntity);
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryDTO categoryDto)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
                throw new KeyNotFoundException("Category not found.");

            _mapper.Map(categoryDto, existingCategory); // Maps the changes to the entity
            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();

            return _mapper.Map<CategoryDTO>(existingCategory);
        }

        public async Task<int> DeleteMultipleCategoriesAsync(IEnumerable<int> ids)
        {
            var categoriesToDelete = await _context.Categories
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            if (!categoriesToDelete.Any())
            {
                _logger.LogWarning("No matching categories found for deletion.");
                return 0;
            }

            _context.Categories.RemoveRange(categoriesToDelete);
            var deletedCount = await _context.SaveChangesAsync();

            return deletedCount;
        }

    }
}
