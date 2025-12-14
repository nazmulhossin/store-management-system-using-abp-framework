using Microsoft.EntityFrameworkCore;
using MyStore.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MyStore.Inventory
{
    public class EfCoreStockRepository : EfCoreRepository<MyStoreDbContext, Stock, Guid>, IStockRepository
    {
        public EfCoreStockRepository(IDbContextProvider<MyStoreDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<Stock>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string? sorting = null,
            string? productName = null,
            string? warehouseName = null)
        {
            var queryable = await GetQueryableAsync();

            queryable = queryable
                .WhereIf(
                    !productName.IsNullOrWhiteSpace(),
                    x => x.ProductName.Contains(productName!)
                )
                .WhereIf(
                    !warehouseName.IsNullOrWhiteSpace(),
                    x => x.WarehouseName.Contains(warehouseName!)
                )
                .Where(x => x.CurrentStock > 0)
                .OrderBy(
                    sorting.IsNullOrWhiteSpace()
                        ? nameof(Stock.ProductName)
                        : sorting
                )
                .Skip(skipCount)
                .Take(maxResultCount);

            return await queryable.ToListAsync();
        }

        public async Task<long> GetCountAsync(
            string? productName = null,
            string? warehouseName = null)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet
                .WhereIf(
                    !productName.IsNullOrWhiteSpace(),
                    x => x.ProductName.Contains(productName!)
                )
                .WhereIf(
                    !warehouseName.IsNullOrWhiteSpace(),
                    x => x.WarehouseName.Contains(warehouseName!)
                )
                .Where(x => x.CurrentStock > 0)
                .LongCountAsync();
        }

        public async Task<Stock?> FindByProductAndWarehouseAsync(
            string productName,
            string warehouseName)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .FirstOrDefaultAsync(x => x.ProductName == productName && x.WarehouseName == warehouseName);
        }
    }
}
