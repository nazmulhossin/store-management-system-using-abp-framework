using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace MyStore.Inventory
{
    public interface IStockRepository : IRepository<Stock, Guid>
    {
        Task<List<Stock>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string? sorting = null,
            string? productName = null,
            string? warehouseName = null
        );

        Task<long> GetCountAsync(
            string? productName = null,
            string? warehouseName = null
        );

        Task<Stock?> FindByProductAndWarehouseAsync(
            string productName,
            string warehouseName
        );
    }
}
