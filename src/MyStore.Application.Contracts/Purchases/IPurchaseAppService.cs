using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace MyStore.Purchases
{
    public interface IPurchaseAppService
    {
        Task<PagedResultDto<PurchaseListDto>> GetListAsync(GetPurchaseListDto input);
        Task<PurchaseDto> CreateAsync(CreateUpdatePurchaseDto input);
    }
}
