using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace MyStore.Purchases
{
    public interface IPurchaseAppService
    {
        Task<PurchaseDto> GetAsync(Guid id);
        Task<PagedResultDto<PurchaseListDto>> GetListAsync(GetPurchaseListDto input);
        Task<PurchaseDto> CreateAsync(CreateUpdatePurchaseDto input);
        Task<PurchaseDto> UpdateAsync(Guid id, CreateUpdatePurchaseDto input);
        Task DeleteAsync(Guid id);
    }
}
