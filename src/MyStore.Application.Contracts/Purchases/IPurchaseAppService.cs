using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using static MyStore.Permissions.MyStorePermissions;

namespace MyStore.Purchases
{
    public interface IPurchaseAppService
    {
        Task<PagedResultDto<PurchaseListDto>> GetListAsync(GetPurchaseListDto input);
        Task<PurchaseDto> CreateAsync(CreateUpdatePurchaseDto input);
    }
}
