using Microsoft.EntityFrameworkCore;
using MyStore.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MyStore.Sales
{
    public class EfCoreSaleRepository : EfCoreRepository<MyStoreDbContext, Sale, Guid>, ISaleRepository
    {
        public EfCoreSaleRepository(IDbContextProvider<MyStoreDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<Sale>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string? customerName = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = await GetQueryableAsync();

            query = ApplyFilter(query, customerName, startDate, endDate);

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

        private static IQueryable<Sale> ApplyFilter(
            IQueryable<Sale> query,
            string? customerName,
            DateTime? startDate,
            DateTime? endDate)
        {
            return query
                .WhereIf(
                    !customerName.IsNullOrWhiteSpace(),
                    x => x.CustomerName.Contains(customerName!)
                )
                .WhereIf(
                    startDate.HasValue,
                    x => x.SalesDate >= startDate!.Value
                )
                .WhereIf(
                    endDate.HasValue,
                    x => x.SalesDate <= endDate!.Value
                );
        }

        public override async Task<IQueryable<Sale>> WithDetailsAsync()
        {
            return (await GetQueryableAsync()).Include(p => p.SaleItems);
        }
    }
}
