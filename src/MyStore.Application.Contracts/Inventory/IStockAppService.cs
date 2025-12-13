using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace MyStore.Inventory
{
    public interface IStockAppService
    {
        Task<PagedResultDto<StockDto>> GetListAsync(GetStockListDto input);
    }
}
