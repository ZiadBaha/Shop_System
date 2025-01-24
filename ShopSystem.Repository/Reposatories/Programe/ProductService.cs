using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Enums;
using ShopSystem.Core.Models.Entites;
using ShopSystem.Core.Services.Programe;
using ShopSystem.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopSystem.Core.Models;

namespace ShopSystem.Repository.Reposatories.Programe
{
    public class ProductService : IProductRepository
    {
        private readonly StoreContext _context; 
        private readonly IMapper _mapper; 
        private readonly ILogger<ProductService> _logger; 

        public ProductService(StoreContext context, IMapper mapper, ILogger<ProductService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<PagedResult<GetProductsDTO>> GetAllProductsAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)  // Include Category to fetch category details
                    .AsQueryable();

                // Apply searching
                if (!string.IsNullOrEmpty(queryOptions.Search))
                {
                    query = query.Where(p => p.Name.Contains(queryOptions.Search) ||
                                             p.UniqueNumber.Contains(queryOptions.Search));
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

                // Map the products to DTOs and include category information
                var productDtos = products.Select(product => new GetProductsDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Quantity = product.Quantity,
                    PurchasePrice = product.PurchasePrice,
                    SellingPrice = product.SellingPrice,
                    CategoryId = product.CategoryId,
                    Status = product.Status ?? ProductStatus.Active,
                    UniqueNumber = product.UniqueNumber,
                    Category = new CategoryDTO
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name
                    }
                }).ToList();

                return new PagedResult<GetProductsDTO>
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

        public async Task<ContentContainer<GetProductsDTO>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)  // Include Category to fetch category details
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found.");
                    return new ContentContainer<GetProductsDTO>(null, $"Product with ID {id} not found.");
                }

                var productDTO = _mapper.Map<GetProductsDTO>(product);  // Map product entity to DTO

                // Manually map the category data
                productDTO.Category = new CategoryDTO
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                };

                return new ContentContainer<GetProductsDTO>(productDTO, "Product retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the product with ID {id}.");
                throw;
            }
        }

        //public async Task<List<Product>> CreateProductsAsync(IEnumerable<ProductDTO> productDtos)
        //{
        //    var products = new List<Product>();

        //    foreach (var productDto in productDtos)
        //    {
        //        // Check if the category exists for each product
        //        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
        //        if (!categoryExists)
        //        {
        //            throw new KeyNotFoundException($"Category not found with the provided ID {productDto.CategoryId}.");
        //        }

        //        // Check for duplicate UniqueNumber
        //        var isUniqueNumberExists = await _context.Products.AnyAsync(p => p.UniqueNumber == productDto.UniqueNumber);
        //        if (isUniqueNumberExists)
        //        {
        //            throw new InvalidOperationException($"The UniqueNumber '{productDto.UniqueNumber}' is already in use.");
        //        }

        //        // Create and add each product to the list
        //        var product = new Product
        //        {
        //            Name = productDto.Name,
        //            Quantity = productDto.Quantity,
        //            IsStock = productDto.IsStock.GetValueOrDefault(),
        //            PurchasePrice = productDto.PurchasePrice,
        //            SellingPrice = productDto.SellingPrice,
        //            CategoryId = productDto.CategoryId,
        //            UniqueNumber = productDto.UniqueNumber,
        //            //Status = productDto.Status ?? ProductStatus.Active, // Default to Active if null
        //        };
        //        products.Add(product);
        //    }

        //    // Add the products to the database and save changes
        //    _context.Products.AddRange(products);
        //    await _context.SaveChangesAsync();
        //    return products;
        //}

        public async Task<List<ProductDTO>> CreateProductsAsync(IEnumerable<ProductDTO> productDtos)
        {
            var products = new List<Product>();

            foreach (var productDto in productDtos)
            {
                // Validate category existence
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
                if (!categoryExists)
                {
                    throw new KeyNotFoundException($"Category not found with the provided ID {productDto.CategoryId}.");
                }

                // Validate unique UniqueNumber
                var isUniqueNumberExists = await _context.Products.AnyAsync(p => p.UniqueNumber == productDto.UniqueNumber);
                if (isUniqueNumberExists)
                {
                    throw new InvalidOperationException($"The UniqueNumber '{productDto.UniqueNumber}' is already in use.");
                }

                // Ensure SellingPrice > PurchasePrice
                if (productDto.SellingPrice <= productDto.PurchasePrice)
                {
                    throw new ArgumentException($"The SellingPrice must be greater than the PurchasePrice for the product '{productDto.Name}'.");
                }

                // Map ProductDTO to Product using AutoMapper
                var product = _mapper.Map<Product>(productDto);
                products.Add(product);
            }

            // Add the products to the database and save changes
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Map saved products back to ProductDTO using AutoMapper
            var createdProductDtos = _mapper.Map<List<ProductDTO>>(products);

            return createdProductDtos;
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
