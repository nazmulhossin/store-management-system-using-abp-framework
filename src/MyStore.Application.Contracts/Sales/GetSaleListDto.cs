using System;
using Volo.Abp.Application.Dtos;

namespace MyStore.Sales
{
    public class GetSaleListDto : PagedAndSortedResultRequestDto
    {
        public string? CustomerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
