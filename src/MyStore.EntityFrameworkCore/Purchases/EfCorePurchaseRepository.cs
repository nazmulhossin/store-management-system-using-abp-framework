using Microsoft.EntityFrameworkCore;
using MyStore.EntityFrameworkCore;
using MyStore.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MyStore.Purchases
{
    public class EfCorePurchaseRepository : EfCoreRepository<MyStoreDbContext, Purchase, Guid>, IPurchaseRepository
    {
        public EfCorePurchaseRepository(IDbContextProvider<MyStoreDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<Purchase>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string? supplierName = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = await GetQueryableAsync();

            query = ApplyFilter(query, supplierName, startDate, endDate);

            return await query
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }

        public async Task<long> GetCountAsync(
            string? customerName = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = await GetQueryableAsync();

            query = ApplyFilter(query, customerName, startDate, endDate);

            return await query.LongCountAsync();
        }

        private static IQueryable<Purchase> ApplyFilter(
            IQueryable<Purchase> query,
            string? customerName,
            DateTime? startDate,
            DateTime? endDate)
        {
            return query
                .WhereIf(
                    !customerName.IsNullOrWhiteSpace(),
                    x => x.SupplierName.Contains(customerName!)
                )
                .WhereIf(
                    startDate.HasValue,
                    x => x.PurchaseDate >= startDate!.Value
                )
                .WhereIf(
                    endDate.HasValue,
                    x => x.PurchaseDate <= endDate!.Value
                );
        }

        public override async Task<IQueryable<Purchase>> WithDetailsAsync()
        {
            return (await GetQueryableAsync()).Include(p => p.PurchaseItems);
        }
    }
}
