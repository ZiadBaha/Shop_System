﻿using Account.Core.Dtos;
using Account.Core.Dtos.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.Services.Programe
{
    public interface ICategoryRepository
    {
        Task<PagedResult<CategoryDTO>> GetAllCategoriesAsync(PaginationParameters paginationParameters);
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDto);
        Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryDTO categoryDto);
        Task<int> DeleteMultipleCategoriesAsync(IEnumerable<int> ids);
    }
}