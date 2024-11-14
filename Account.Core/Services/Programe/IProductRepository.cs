using Account.Core.Dtos;
using Account.Core.Dtos.Program;
using Account.Core.Models.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.Services.Programe
{
    public interface IProductRepository
    {
        //Task<PagedResult<ProductDTO>> GetAllProductsAsync(PaginationParameters paginationParameters);
        Task<PagedResult<ProductDTO>> GetAllProductsAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);

        Task<ProductDTO> GetProductByIdAsync(int id);
        //Task<ProductDTO> CreateProductAsync(ProductDTO productDto);
        Task<List<Product>> CreateProductsAsync(IEnumerable<ProductDTO> productDtos);

        Task<ProductDTO> UpdateProductAsync(int id, ProductDTO productDto);
        Task<int> DeleteMultipleProductsAsync(IEnumerable<int> ids);
        //Task<bool> UpdateProductStockAsync(int productId, int quantityChange);
        Task UpdateProductStockAsync(int productId, int quantityChange);
        Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId);
        Task<int?> GetAvailableStockAsync(int productId);




    }
}
