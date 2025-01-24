using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Models.Entites;
using ShopSystem.Core.Services.Programe;
using ShopSystem.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Repository.Reposatories.Programe
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


        public async Task<PagedResult<GetPurchaseDTO>> GetAllPurchasesAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            try
            {
                var query = _context.Purchases
                    .Include(p => p.PurchaseItems) // Include Purchase Items
                    .Include(p => p.Merchant)     // Include Merchant details
                    .AsQueryable();

                // Apply searching
                if (!string.IsNullOrEmpty(queryOptions.Search))
                {
                    query = query.Where(p =>
                        p.Notes.Contains(queryOptions.Search) ||
                        p.Merchant.Name.Contains(queryOptions.Search) ||
                        p.Id.ToString().Contains(queryOptions.Search));
                }

                // Apply filtering by MinAmount and MaxAmount
                if (queryOptions.MinAmount.HasValue)
                {
                    query = query.Where(p => p.TotalAmount >= queryOptions.MinAmount.Value);
                }
                if (queryOptions.MaxAmount.HasValue)
                {
                    query = query.Where(p => p.TotalAmount <= queryOptions.MaxAmount.Value);
                }

                // Apply filtering by StartDate and EndDate
                if (queryOptions.StartDate.HasValue)
                {
                    query = query.Where(p => p.OrderDate >= queryOptions.StartDate.Value);
                }
                if (queryOptions.EndDate.HasValue)
                {
                    query = query.Where(p => p.OrderDate <= queryOptions.EndDate.Value);
                }

                // Apply sorting
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

                // Map the purchases to DTOs
                var purchaseDtos = purchases.Select(purchase => new GetPurchaseDTO
                {
                    Id = purchase.Id,
                    TotalAmount = purchase.TotalAmount,
                    OrderDate = purchase.OrderDate,
                    Notes = purchase.Notes,
                    Merchant = new MerchantDTO
                    {
                        Id = purchase.Merchant.Id,
                        Name = purchase.Merchant.Name
                    },
                    PurchaseItems = purchase.PurchaseItems.Select(item => new PurchaseItemDTO
                    {
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                }).ToList();

                return new PagedResult<GetPurchaseDTO>
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


        public async Task<GetPurchaseDTO> GetPurchaseByIdAsync(int purchaseId)
        {
            try
            {
                // Retrieve the purchase with its related Merchant and PurchaseItems
                var purchase = await _context.Purchases
                    .Include(p => p.PurchaseItems)
                    .Include(p => p.Merchant) // Include Merchant data
                    .FirstOrDefaultAsync(p => p.Id == purchaseId);

                if (purchase == null)
                {
                    _logger.LogWarning($"Purchase with ID {purchaseId} not found.");
                    return null;
                }

                // Map to GetPurchaseDTO
                var result = new GetPurchaseDTO
                {
                    Id = purchase.Id,
                    OrderDate = purchase.OrderDate,
                    TotalAmount = purchase.TotalAmount,
                    Notes = purchase.Notes,
                    PurchaseItems = purchase.PurchaseItems.Select(pi => new PurchaseItemDTO
                    {
                        Id = pi.Id,
                        ProductName = pi.ProductName,
                        Quantity = pi.Quantity,
                        TotalPrice = pi.TotalPrice
                    }).ToList(),
                    Merchant = new MerchantDTO
                    {
                        Id = purchase.Merchant.Id,
                        Name = purchase.Merchant.Name
                    }
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving purchase with ID {purchaseId}.");
                throw;
            }
        }

        public async Task<PurchaseDTO> CreatePurchaseAsync(CreatePurchaseDTO purchaseDto)
        {
            try
            {
                // Map Purchase
                var purchase = _mapper.Map<Purchase>(purchaseDto);

                if (purchaseDto.PurchaseItems == null || !purchaseDto.PurchaseItems.Any())
                {
                    throw new InvalidOperationException("PurchaseItems cannot be empty.");
                }

                purchase.TotalAmount = 0;

                // Calculate TotalPrice for each PurchaseItem and accumulate TotalAmount
                foreach (var item in purchase.PurchaseItems)
                {
                    item.TotalPrice = item.Quantity * item.PricePerUnit; // Correctly calculate TotalPrice
                    purchase.TotalAmount += item.TotalPrice; // Add to TotalAmount
                }

                // Save Purchase to the database
                await _context.Purchases.AddAsync(purchase);
                await _context.SaveChangesAsync();

                // Fetch saved data (to include generated IDs)
                var savedPurchase = await _context.Purchases
                    .Include(p => p.PurchaseItems)
                    .FirstOrDefaultAsync(p => p.Id == purchase.Id);

                if (savedPurchase == null)
                {
                    throw new InvalidOperationException("Failed to retrieve saved purchase.");
                }

                // Map to DTO
                var purchaseDtoResult = _mapper.Map<PurchaseDTO>(savedPurchase);

                return purchaseDtoResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a purchase.");
                throw;
            }
        }

        public async Task<bool> UpdatePurchaseDetailsAsync(int purchaseId, CreatePurchaseDTO purchaseDto)
        {
            try
            {
                var purchase = await _context.Purchases.Include(p => p.PurchaseItems).FirstOrDefaultAsync(p => p.Id == purchaseId);

                if (purchase == null)
                {
                    _logger.LogWarning($"Purchase with ID {purchaseId} not found for update.");
                    return false;
                }

                // Remove existing purchase items and map new data
                _context.PurchaseItems.RemoveRange(purchase.PurchaseItems);
                var updatedPurchase = _mapper.Map(purchaseDto, purchase);
                updatedPurchase.TotalAmount = updatedPurchase.PurchaseItems.Sum(item => item.PricePerUnit * item.Quantity);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating purchase with ID {purchaseId}.");
                throw;
            }
        }

        public async Task<int> DeleteMultiplePurchasesAsync(IEnumerable<int> ids)
        {
            try
            {
                var purchases = await _context.Purchases
                    .Where(p => ids.Contains(p.Id))
                    .ToListAsync();

                if (!purchases.Any())
                {
                    _logger.LogWarning("No matching purchases found for deletion.");
                    return 0;
                }

                _context.Purchases.RemoveRange(purchases);
                var deletedCount = await _context.SaveChangesAsync();
                _logger.LogInformation($"{deletedCount} purchases deleted successfully.");
                return deletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting multiple purchases.");
                throw;
            }
        }


        public async Task<decimal> GetPurchaseTotalAmountAsync(int purchaseId)
        {
            try
            {
                var purchase = await _context.Purchases
                    .Include(p => p.PurchaseItems)  // Make sure PurchaseItems are included
                    .FirstOrDefaultAsync(p => p.Id == purchaseId);

                if (purchase == null)
                {
                    _logger.LogWarning($"Purchase with ID {purchaseId} not found.");
                    return 0;  // Or throw exception, depending on your desired behavior
                }

                // Sum of PricePerUnit * Quantity for each PurchaseItem
                var totalAmount = purchase.PurchaseItems.Sum(item => item.PricePerUnit * item.Quantity);

                return totalAmount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while calculating the total amount for purchase ID {purchaseId}.");
                throw;
            }
        }


    }
}

