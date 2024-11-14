using Account.Core.Dtos.Program;
using Account.Core.Dtos;
using Account.Core.Models.Entites;
using Account.Core.Services.Programe;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Account.Reposatory.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Account.Reposatory.Reposatories.Programe
{
    public class PurchaseService : IPurchaseRepository
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PurchaseService> _logger;

        public PurchaseService(StoreContext context, IMapper mapper, ILogger<PurchaseService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<PagedResult<PurchaseDTO>> GetAllPurchasesAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            try
            {
                var query = _context.Purchases.AsQueryable();

                // Apply searching by MerchantId, Merchant name, or other relevant fields
                if (!string.IsNullOrEmpty(queryOptions.Search))
                {
                    query = query.Include(p => p.Merchant); // Include the Merchant entity in the query

                    // Check if the search term is a number to filter by MerchantId
                    if (int.TryParse(queryOptions.Search, out int merchantId))
                    {
                        query = query.Where(p => p.MerchantId == merchantId);
                    }
                    else
                    {
                        // Otherwise, search by Merchant's Name
                        query = query.Where(p => p.Merchant.Name.Contains(queryOptions.Search));
                    }
                }


                // Apply filtering by MinAmount and MaxAmount if available
                if (queryOptions.MinAmount.HasValue)
                {
                    query = query.Where(p => p.TotalAmount >= queryOptions.MinAmount.Value);
                }
                if (queryOptions.MaxAmount.HasValue)
                {
                    query = query.Where(p => p.TotalAmount <= queryOptions.MaxAmount.Value);
                }

                // Apply sorting if the specified SortField exists on the Purchase entity
                if (!string.IsNullOrEmpty(queryOptions.SortField))
                {
                    var propertyInfo = typeof(Purchase).GetProperty(queryOptions.SortField);
                    if (propertyInfo != null)
                    {
                        query = queryOptions.SortDescending
                            ? query.OrderByDescending(e => EF.Property<object>(e, queryOptions.SortField))
                            : query.OrderBy(e => EF.Property<object>(e, queryOptions.SortField));
                    }
                    else
                    {
                        _logger.LogWarning($"Sort field '{queryOptions.SortField}' does not exist on Purchase entity.");
                    }
                }

                // Get total count for pagination
                var totalItems = await query.CountAsync();

                // Apply pagination
                var purchases = await query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                var purchaseDtos = _mapper.Map<List<PurchaseDTO>>(purchases);

                return new PagedResult<PurchaseDTO>
                {
                    Items = purchaseDtos,
                    TotalCount = totalItems,
                    PageNumber = paginationParameters.PageNumber,
                    PageSize = paginationParameters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all purchases.");
                throw;
            }
        }


        #region Get All
        //public async Task<PagedResult<PurchaseDTO>> GetAllPurchasesAsync(PaginationParameters paginationParameters)
        //{
        //    try
        //    {
        //        var query = _context.Purchases.AsQueryable();
        //        var totalCount = await query.CountAsync();

        //        var purchases = await query
        //            .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
        //            .Take(paginationParameters.PageSize)
        //            .ToListAsync();

        //        var purchaseDtos = _mapper.Map<IEnumerable<PurchaseDTO>>(purchases);

        //        return new PagedResult<PurchaseDTO>
        //        {
        //            Items = purchaseDtos,
        //            TotalCount = totalCount,
        //            PageNumber = paginationParameters.PageNumber,
        //            PageSize = paginationParameters.PageSize
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while fetching paginated purchases.");
        //        throw;
        //    }
        //} 
        #endregion

        public async Task<PurchaseDTO> GetPurchaseByIdAsync(int id)
        {
            try
            {
                var purchase = await _context.Purchases.FindAsync(id);
                if (purchase == null)
                {
                    _logger.LogWarning($"Purchase with ID {id} not found.");
                    throw new KeyNotFoundException($"Purchase with ID {id} not found.");
                }
                return _mapper.Map<PurchaseDTO>(purchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the purchase with ID {id}.");
                throw;
            }
        }

        #region Create 
        //public async Task<PurchaseDTO> CreatePurchaseAsync(PurchaseDTO purchaseDto, bool isReturn = false)
        //{
        //    try
        //    {
        //        var purchase = _mapper.Map<Purchase>(purchaseDto);

        //        // For each product in the purchase or return, adjust stock levels
        //        foreach (var purchasedProduct in purchase.Products)
        //        {
        //            // Fetch the product from the database to get current stock
        //            var product = await _context.Products.FindAsync(purchasedProduct.ProductId);
        //            if (product == null)
        //            {
        //                throw new KeyNotFoundException($"Product with ID {purchasedProduct.ProductId} not found.");
        //            }

        //            // Adjust the stock quantity based on purchase or return
        //            if (isReturn)
        //            {
        //                // Increase the stock quantity for returns
        //                product.Quantity += purchasedProduct.Quantity;
        //            }
        //            else
        //            {
        //                // Reduce the stock quantity for purchases
        //                if (product.Quantity < purchasedProduct.Quantity)
        //                {
        //                    throw new InvalidOperationException($"Insufficient stock for product ID {purchasedProduct.ProductId}.");
        //                }

        //                product.Quantity -= purchasedProduct.Quantity;
        //            }

        //            // Update the product in the database
        //            _context.Products.Update(product);
        //        }

        //        // Add the purchase (or return record) to the database
        //        await _context.Purchases.AddAsync(purchase);
        //        await _context.SaveChangesAsync();

        //        return _mapper.Map<PurchaseDTO>(purchase);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while creating a new purchase or processing a return.");
        //        throw;
        //    }
        //} 
        #endregion

        public async Task<PurchaseDTO> CreatePurchaseAsync(PurchaseDTO purchaseDto, bool isReturn = false)
        {
            try
            {
                var purchase = _mapper.Map<Purchase>(purchaseDto);

                if (purchase.Products == null || !purchase.Products.Any())
                {
                    throw new ArgumentException("Purchase must contain at least one product.");
                }

                foreach (var purchasedProduct in purchase.Products)
                {
                    // Fetch the product from the database using the ProductId in PurchaseItem
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == purchasedProduct.ProductId);

                    if (product == null)
                    {
                        // If the product doesn't exist, create a new one
                        product = new Product
                        {
                            Id = purchasedProduct.ProductId, // Ensure ProductId is set
                            Quantity = purchasedProduct.Quantity
                        };

                        _context.Products.Add(product);
                    }
                    else
                    {
                        // If the product exists, adjust the stock
                        if (isReturn)
                        {
                            // Increase the stock for returns
                            product.Quantity += purchasedProduct.Quantity;
                        }
                        else
                        {
                            // Ensure enough stock is available for purchase
                            if (product.Quantity < purchasedProduct.Quantity)
                            {
                                throw new InvalidOperationException($"Insufficient stock for product with ID {product.Id}.");
                            }

                            // Decrease the stock for purchase
                            product.Quantity -= purchasedProduct.Quantity;
                        }

                        _context.Products.Update(product);
                    }
                }

                // Add the purchase to the database
                await _context.Purchases.AddAsync(purchase);
                await _context.SaveChangesAsync();

                return _mapper.Map<PurchaseDTO>(purchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new purchase or processing a return.");
                throw;
            }
        }




        public async Task<PurchaseDTO> UpdatePurchaseAsync(int id, PurchaseDTO purchaseDto)
        {
            try
            {
                var purchase = await _context.Purchases.FindAsync(id);
                if (purchase == null)
                {
                    _logger.LogWarning($"Purchase with ID {id} not found for update.");
                    throw new KeyNotFoundException($"Purchase with ID {id} not found.");
                }

                _mapper.Map(purchaseDto, purchase);
                _context.Purchases.Update(purchase);
                await _context.SaveChangesAsync();
                return _mapper.Map<PurchaseDTO>(purchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the purchase with ID {id}.");
                throw;
            }
        }

        #region Delete
        //public async Task<bool> DeletePurchaseAsync(int id)
        //{
        //    try
        //    {
        //        var purchase = await _context.Purchases.FindAsync(id);
        //        if (purchase == null)
        //        {
        //            _logger.LogWarning($"Purchase with ID {id} not found for deletion.");
        //            throw new KeyNotFoundException($"Purchase with ID {id} not found.");
        //        }

        //        _context.Purchases.Remove(purchase);
        //        await _context.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"An error occurred while deleting the purchase with ID {id}.");
        //        throw;
        //    }
        //} 
        #endregion
        public async Task<bool> DeletePurchaseAsync(int id, bool isReturn = false)
        {
            try
            {
                // Find the purchase
                var purchase = await _context.Purchases.Include(p => p.Products).FirstOrDefaultAsync(p => p.Id == id);

                if (purchase == null)
                {
                    _logger.LogWarning($"Purchase with ID {id} not found for deletion.");
                    throw new KeyNotFoundException($"Purchase with ID {id} not found.");
                }

                // For each product in the purchase, adjust stock levels
                foreach (var purchasedProduct in purchase.Products)
                {
                    var product = await _context.Products.FindAsync(purchasedProduct.ProductId);
                    if (product == null)
                    {
                        throw new KeyNotFoundException($"Product with ID {purchasedProduct.ProductId} not found.");
                    }

                    if (isReturn)
                    {
                        // If it's a return, increase stock
                        product.Quantity += purchasedProduct.Quantity;
                    }
                    else
                    {
                        // If it's a deletion, increase stock as if the sale is reversed
                        product.Quantity += purchasedProduct.Quantity;
                    }

                    // Update the product stock
                    _context.Products.Update(product);
                }

                // Remove the purchase from the database
                _context.Purchases.Remove(purchase);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the purchase with ID {id}.");
                throw;
            }
        }


        public async Task<IEnumerable<ProductDTO>> GetProductsInPurchaseAsync(int purchaseId)
        {
            try
            {
                var purchase = await _context.Purchases
                    .Include(p => p.Products)
                    .FirstOrDefaultAsync(p => p.Id == purchaseId);

                if (purchase == null)
                {
                    _logger.LogWarning($"Purchase with ID {purchaseId} not found.");
                    throw new KeyNotFoundException($"Purchase with ID {purchaseId} not found.");
                }

                return _mapper.Map<IEnumerable<ProductDTO>>(purchase.Products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching products for purchase ID {purchaseId}.");
                throw;
            }
        }

        public async Task<decimal> CalculateTotalPurchaseAmountAsync(int purchaseId)
        {
            try
            {
                var purchase = await _context.Purchases
                    .Include(p => p.Products)
                    .FirstOrDefaultAsync(p => p.Id == purchaseId);

                if (purchase == null)
                {
                    _logger.LogWarning($"Purchase with ID {purchaseId} not found.");
                    throw new KeyNotFoundException($"Purchase with ID {purchaseId} not found.");
                }

                return purchase.Products.Sum(p => p.PricePerUnit * p.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while calculating total amount for purchase ID {purchaseId}.");
                throw;
            }
        }
    }
}
