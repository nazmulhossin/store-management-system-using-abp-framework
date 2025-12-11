using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace MyStore.Purchases
{
    public interface IPurchaseRepository : IRepository<Purchase, Guid>
    {
        Task<List<Purchase>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null
        );
    }
}
