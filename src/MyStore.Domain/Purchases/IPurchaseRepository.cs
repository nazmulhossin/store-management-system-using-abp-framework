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
            string? supplierName = null,
            DateTime? startDate = null,
            DateTime? endDate = null
        );

        Task<long> GetCountAsync(
            string? customerName = null,
            DateTime? startDate = null,
            DateTime? endDate = null
        );
    }
}
