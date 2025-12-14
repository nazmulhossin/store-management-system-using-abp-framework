using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace MyStore.Sales
{
    public interface ISaleAppService
    {
        Task<SaleDto> GetAsync(Guid id);
        Task<PagedResultDto<SaleListDto>> GetListAsync(GetSaleListDto input);
        Task<SaleDto> CreateAsync(CreateUpdateSaleDto input);
        Task<SaleDto> UpdateAsync(Guid id, CreateUpdateSaleDto input);
        Task DeleteAsync(Guid id);
    }
}
