using Volo.Abp.Application.Dtos;

namespace MyStore.Purchases
{
    public class GetPurchaseListDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }
}
