using System;
using Volo.Abp.Application.Dtos;

namespace MyStore.Purchases
{
    public class GetPurchaseListDto : PagedAndSortedResultRequestDto
    {
        public string? SupplierName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
