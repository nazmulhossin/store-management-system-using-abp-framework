using Microsoft.EntityFrameworkCore;
using MyStore.EntityFrameworkCore;
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
            string filter = null)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }

        public override async Task<IQueryable<Purchase>> WithDetailsAsync()
        {
            return (await GetQueryableAsync()).Include(p => p.PurchaseItems);
        }
    }
}
