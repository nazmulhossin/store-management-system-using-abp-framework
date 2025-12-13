using Microsoft.AspNetCore.Authorization;
using MyStore.Permissions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace MyStore.Inventory
{
    [Authorize(MyStorePermissions.Inventory.Default)]
    public class StockAppService : MyStoreAppService, IStockAppService
    {
        private readonly IStockRepository _stockRepository;

        public StockAppService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<PagedResultDto<StockDto>> GetListAsync(GetStockListDto input)
        {
            var stocks = await _stockRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.ProductName,
                input.WarehouseName
            );

            var totalCount = await _stockRepository.GetCountAsync(
                input.ProductName,
                input.WarehouseName
            );

            return new PagedResultDto<StockDto>(
                totalCount,
                ObjectMapper.Map<List<Stock>, List<StockDto>>(stocks)
            );
        }
    }
}
