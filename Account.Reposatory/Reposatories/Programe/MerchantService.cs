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
            var query = _context.Merchants.AsQueryable();

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

            if (!string.IsNullOrEmpty(queryOptions.SortField))
            {
                var propertyInfo = typeof(Merchant).GetProperty(queryOptions.SortField);
                if (propertyInfo != null)
                {
                    query = queryOptions.SortDescending
                        ? query.OrderByDescending(e => EF.Property<object>(e, queryOptions.SortField))
                        : query.OrderBy(e => EF.Property<object>(e, queryOptions.SortField));
                }
            }

            var totalCount = await query.CountAsync();
            var merchants = await query
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync();

            var merchantDtos = _mapper.Map<IEnumerable<MerchantDTO>>(merchants);

            return new PagedResult<MerchantDTO>
            {
                Items = merchantDtos,
                TotalCount = totalCount,
                PageNumber = paginationParameters.PageNumber,
                PageSize = paginationParameters.PageSize
            };
        }

        public async Task<MerchantDTO> GetMerchantByIdAsync(int id)
        {
            var merchant = await _context.Merchants.FindAsync(id);
            return merchant != null ? _mapper.Map<MerchantDTO>(merchant) : throw new KeyNotFoundException("Merchant not found.");
        }

        public async Task<CreateMerchantDTO> CreateMerchantAsync(CreateMerchantDTO createMerchantDto)
        {
            var merchantEntity = _mapper.Map<Merchant>(createMerchantDto);
            await _context.Merchants.AddAsync(merchantEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<CreateMerchantDTO>(merchantEntity);
        }

        public async Task<MerchantDTO> UpdateMerchantAsync(int id, MerchantDTO merchantDto)
        {
            var existingMerchant = await _context.Merchants.FindAsync(id);
            if (existingMerchant == null) throw new KeyNotFoundException("Merchant not found.");

            _mapper.Map(merchantDto, existingMerchant);
            _context.Merchants.Update(existingMerchant);
            await _context.SaveChangesAsync();

            return _mapper.Map<MerchantDTO>(existingMerchant);
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

        public async Task<decimal> CalculateOutstandingBalanceAsync(int merchantId)
        {
            return await _context.Purchases
                .Where(p => p.MerchantId == merchantId && !p.IsPaid)
                .SumAsync(p => p.Amount);
        }

        public async Task<IEnumerable<PurchaseDTO>> GetMerchantPurchasesAsync(int merchantId)
        {
            var purchases = await _context.Purchases.Where(p => p.MerchantId == merchantId).ToListAsync();
            return _mapper.Map<IEnumerable<PurchaseDTO>>(purchases);
        }
    }

}
