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
    public class MerchantService : IMerchantRepository
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<MerchantService> _logger;

        public MerchantService(StoreContext context, IMapper mapper, ILogger<MerchantService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<MerchantDTO>> GetAllMerchantsAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            var query = _context.Merchants.Include(m => m.Purchases).AsQueryable();

            // Filtering by Search
            if (!string.IsNullOrEmpty(queryOptions.Search))
            {
                if (int.TryParse(queryOptions.Search, out int merchantId))
                {
                    query = query.Where(m => m.Id == merchantId);
                }
                else
                {
                    query = query.Where(m => m.Name.Contains(queryOptions.Search));
                }
            }

            // Sorting
            if (!string.IsNullOrEmpty(queryOptions.SortField))
            {
                query = queryOptions.SortField switch
                {
                    nameof(Merchant.Name) => queryOptions.SortDescending
                        ? query.OrderByDescending(m => m.Name)
                        : query.OrderBy(m => m.Name),

                    nameof(Merchant.Phone) => queryOptions.SortDescending
                        ? query.OrderByDescending(m => m.Phone)
                        : query.OrderBy(m => m.Phone),

                    nameof(Merchant.Address) => queryOptions.SortDescending
                        ? query.OrderByDescending(m => m.Address)
                        : query.OrderBy(m => m.Address),

                    nameof(Merchant.OutstandingBalance) => queryOptions.SortDescending
                       ? query.OrderByDescending(m => m.OutstandingBalance)
                       : query.OrderBy(m => m.OutstandingBalance),

                    "TotalPurchaseAmount" => queryOptions.SortDescending
                        ? query.OrderByDescending(m => m.Purchases.Sum(p => p.TotalAmount))
                        : query.OrderBy(m => m.Purchases.Sum(p => p.TotalAmount)),



                    _ => query // Default case: No sorting
                };
            }

            // Total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var merchants = await query
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync();

            // Map to DTO
            var merchantDtos = _mapper.Map<IEnumerable<MerchantDTO>>(merchants);

            // Return paged result
            return new PagedResult<MerchantDTO>
            {
                Items = merchantDtos,
                TotalCount = totalCount,
                PageNumber = paginationParameters.PageNumber,
                PageSize = paginationParameters.PageSize
            };
        }

        //public async Task<MerchantDTO> GetMerchantByIdAsync(int id)
        //{
        //    var merchant = await _context.Merchants.FindAsync(id);
        //    return merchant != null ? _mapper.Map<MerchantDTO>(merchant) : throw new KeyNotFoundException("Merchant not found.");
        //}
        public async Task<MerchantDTO> GetMerchantByIdAsync(int id)
        {
            var merchant = await _context.Merchants.FindAsync(id);
            if (merchant == null)
            {
                _logger.LogWarning($"Merchant with ID {id} not found.");
                throw new KeyNotFoundException($"Merchant with ID {id} not found.");
            }

            return _mapper.Map<MerchantDTO>(merchant);
        }

        public async Task<CreateMerchantDTO> CreateMerchantAsync(CreateMerchantDTO createMerchantDto)
        {
            var merchantEntity = _mapper.Map<Merchant>(createMerchantDto);
            await _context.Merchants.AddAsync(merchantEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<CreateMerchantDTO>(merchantEntity);
        }

        public async Task<bool> IsPhoneOrNameDuplicatedAsync(string phone, string name)
        {
            return await _context.Merchants.AnyAsync(m => m.Phone == phone || m.Name == name);
        }
        public async Task<UpdateMerchantDTO> UpdateMerchantAsync(int id, UpdateMerchantDTO updateMerchantDTO)
        {
            var existingMerchant = await _context.Merchants.FindAsync(id);
            if (existingMerchant == null) throw new KeyNotFoundException("Merchant not found."); 

            _mapper.Map(updateMerchantDTO, existingMerchant);
            _context.Merchants.Update(existingMerchant);
            await _context.SaveChangesAsync();

            return _mapper.Map<UpdateMerchantDTO>(existingMerchant);
        }


        public async Task<(int deletedCount, string message)> DeleteMultipleMerchantsAsync(IEnumerable<int> ids)
        {
            var merchants = await _context.Merchants
                .Include(m => m.Purchases)
                .Where(m => ids.Contains(m.Id))
                .ToListAsync();

            if (!merchants.Any())
                return (0, "No matching merchants found to delete.");

            var merchantsToDelete = merchants.Where(m => !m.Purchases.Any()).ToList();

            if (!merchantsToDelete.Any())
                return (0, "All selected merchants have associated purchases. Please remove related purchases first.");

            _context.Merchants.RemoveRange(merchantsToDelete);
            await _context.SaveChangesAsync();

            return (merchantsToDelete.Count, $"{merchantsToDelete.Count} merchants deleted successfully.");
        }

        //public async Task<decimal> CalculateOutstandingBalanceAsync(int merchantId)
        //{
        //    return await _context.Purchases
        //        .Where(p => p.MerchantId == merchantId && !p.IsPaid)
        //        .SumAsync(p => p.Amount);
        //}

        public async Task<IEnumerable<PurchaseDTO>> GetMerchantPurchasesAsync(int merchantId)
        {
            var purchases = await _context.Purchases.Where(p => p.MerchantId == merchantId).ToListAsync();
            return _mapper.Map<IEnumerable<PurchaseDTO>>(purchases);
        }
    }

}
