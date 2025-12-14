using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace MyStore.Sales
{
    public interface ISaleRepository : IRepository<Sale, Guid>
    {
        Task<List<Sale>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string? customerName = null,
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
