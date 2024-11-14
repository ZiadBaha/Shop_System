using Account.Core.Dtos.Program;
using Account.Core.Dtos;
using Account.Core.Models.Entites;
using Account.Core.Services.Programe;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Account.Reposatory.Data;
using Microsoft.EntityFrameworkCore;
using Account.Core.Enums;

namespace Account.Reposatory.Reposatories.Programe
{
    public class ProductService : IProductRepository
    {
        private readonly StoreContext _context; // Replace with your actual DbContext
        private readonly IMapper _mapper; // AutoMapper for mapping between entities and DTOs
        private readonly ILogger<ProductService> _logger; // Logger for logging

        public ProductService(StoreContext context, IMapper mapper, ILogger<ProductService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        #region Get All Products 
        //public async Task<PagedResult<ProductDTO>> GetAllProductsAsync(PaginationParameters paginationParameters)
        //{
        //    try
        //    {
        //        var query = _context.Products.AsQueryable(); // Assuming Products is your DbSet<Product>

        //        // Apply pagination
        //        var totalItems = await query.CountAsync();
        //        var products = await query
        //            .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
        //            .Take(paginationParameters.PageSize)
        //            .ToListAsync();

        //        var productDtos = _mapper.Map<List<ProductDTO>>(products); // Map entities to DTOs
        //        return new PagedResult<ProductDTO>
        //        {
        //            Items = productDtos,
        //            TotalCount = totalItems,
        //            PageNumber = paginationParameters.PageNumber,
        //            PageSize = paginationParameters.PageSize
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while fetching all products.");
        //        throw; // Rethrow the exception to be handled by the caller
        //    }
        //} 
        #endregion
        



        public async Task<PagedResult<ProductDTO>> GetAllProductsAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            try
            {
                var query = _context.Products.AsQueryable();

                // Apply searching
                if (!string.IsNullOrEmpty(queryOptions.Search))
                {
                    query = query.Where(p => p.Name.Contains(queryOptions.Search));
                }

                // Apply filtering by MinPrice and MaxPrice if available
                if (queryOptions.MinAmount.HasValue)
                {
                    query = query.Where(p => p.PurchasePrice >= queryOptions.MinAmount.Value);
                }
                if (queryOptions.MaxAmount.HasValue)
                {
                    query = query.Where(p => p.PurchasePrice <= queryOptions.MaxAmount.Value);
                }

                // Apply sorting if the specified SortField exists on the Product entity
                if (!string.IsNullOrEmpty(queryOptions.SortField))
                {
                    var propertyInfo = typeof(Product).GetProperty(queryOptions.SortField);
                    if (propertyInfo != null)
                    {
                        query = queryOptions.SortDescending
                            ? query.OrderByDescending(e => EF.Property<object>(e, queryOptions.SortField))
                            : query.OrderBy(e => EF.Property<object>(e, queryOptions.SortField));
                    }
                    else
                    {
                        _logger.LogWarning($"Sort field '{queryOptions.SortField}' does not exist on Product entity.");
                    }
                }

                // Get total count for pagination
                var totalItems = await query.CountAsync();

                // Apply pagination
                var products = await query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                var productDtos = _mapper.Map<List<ProductDTO>>(products);

                return new PagedResult<ProductDTO>
                {
                    Items = productDtos,
                    TotalCount = totalItems,
                    PageNumber = paginationParameters.PageNumber,
                    PageSize = paginationParameters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all products.");
                throw;
            }
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found.");
                    return null; // Or throw a custom exception if needed
                }

                return _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the product with ID {id}.");
                throw; // Rethrow the exception to be handled by the caller
            }
        }

       
        public async Task<List<Product>> CreateProductsAsync(IEnumerable<ProductDTO> productDtos)
        {
            var products = new List<Product>();

            foreach (var productDto in productDtos)
            {
                // Check if the category exists for each product
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
                if (!categoryExists)
                {
                    throw new KeyNotFoundException($"Category not found with the provided ID {productDto.CategoryId}.");
                }

                // Create and add each product to the list
                var product = new Product
                {
                    Name = productDto.Name,
                    Quantity = productDto.Quantity,
                    IsStock = productDto.IsStock.GetValueOrDefault(),
                    PurchasePrice = productDto.PurchasePrice,
                    SellingPrice = productDto.SellingPrice,
                    CategoryId = productDto.CategoryId,
                    Status = productDto.Status ?? ProductStatus.Active, // Default to Active if null
                };
                products.Add(product);
            }

            // Add the products to the database and save changes
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();
            return products;
        }



        public async Task<ProductDTO> UpdateProductAsync(int id, ProductDTO productDto)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found for update.");
                    return null; // Or throw a custom exception if needed
                }

                _mapper.Map(productDto, product); // Update product properties with DTO

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return productDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the product with ID {id}.");
                throw; // Rethrow the exception to be handled by the caller
            }
        }

        public async Task<int> DeleteMultipleProductsAsync(IEnumerable<int> ids)
        {
            var products = await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            if (!products.Any())
            {
                _logger.LogWarning("No matching products found for deletion.");
                return 0;
            }

            _context.Products.RemoveRange(products);
            var deletedCount = await _context.SaveChangesAsync();
            _logger.LogInformation($"{deletedCount} products deleted successfully.");
            return deletedCount;
        }


        public async Task UpdateProductStockAsync(int productId, int quantityChange)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {productId} not found for stock update.");
                    throw new KeyNotFoundException("Product not found.");
                }

                // Update the stock quantity
                product.Quantity += quantityChange;

                // Check if quantity is zero or less, set IsStock accordingly
                product.IsStock = product.Quantity > 0;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the stock for product ID {productId}.");
                throw; // Rethrow the exception to be handled by the caller
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.CategoryId == categoryId) // Assuming CategoryId exists
                    .ToListAsync();

                return _mapper.Map<List<ProductDTO>>(products); // Map entities to DTOs
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching products for category ID {categoryId}.");
                throw; // Rethrow the exception to be handled by the caller
            }
        }

        public async Task<int?> GetAvailableStockAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {productId} not found.");
                    return null; // Returning null if product not found
                }

                return product.Quantity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching available stock for product ID {productId}.");
                throw; // Rethrow the exception to be handled by the caller
            }
        }
    }
}
