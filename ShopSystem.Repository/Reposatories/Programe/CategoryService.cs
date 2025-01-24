using AutoMapper;
using Microsoft.Extensions.Logging;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Models.Entites;
using ShopSystem.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopSystem.Core.Services.Programe;
using Microsoft.EntityFrameworkCore;

namespace ShopSystem.Repository.Reposatories.Programe
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

      
        public async Task<PagedResult<CategoryDTO>> GetAllCategoriesAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            try
            {
                var query = _context.Categories.AsQueryable();

                if (!string.IsNullOrEmpty(queryOptions.Search))
                {
                    query = query.Where(c => c.Name.Contains(queryOptions.Search));
                }

                query = queryOptions.SortField.ToLower() switch
                {
                    "id" => queryOptions.SortDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                    "name" => queryOptions.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                    _ => queryOptions.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name), // Default sorting by Name
                };

                var totalCount = await query.CountAsync();

                var categories = await query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                var categoryDtos = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

                return new PagedResult<CategoryDTO>
                {
                    Items = categoryDtos,
                    TotalCount = totalCount,
                    PageNumber = paginationParameters.PageNumber,
                    PageSize = paginationParameters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all categories.");
                throw;
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

            _mapper.Map(categoryDto, existingCategory); 
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
