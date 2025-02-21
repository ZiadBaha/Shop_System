﻿using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Models.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopSystem.Core.Models;

namespace ShopSystem.Core.Services.Programe
{
    public interface IProductRepository
    {
        Task<PagedResult<GetProductsDTO>> GetAllProductsAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);

        Task<ContentContainer<GetProductsDTO>> GetProductByIdAsync(int id);
        Task<List<ProductDTO>> CreateProductsAsync(IEnumerable<ProductDTO> productDtos);

        Task<ProductDTO> UpdateProductAsync(int id, ProductDTO productDto);
        Task<int> DeleteMultipleProductsAsync(IEnumerable<int> ids);
        Task UpdateProductStockAsync(int productId, int quantityChange);
        Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId);
        Task<int?> GetAvailableStockAsync(int productId);
        Task<ContentContainer<GetProductsDTO>> GetProductByUniqueNumberAsync(string uniqueNumber);


    }
}
