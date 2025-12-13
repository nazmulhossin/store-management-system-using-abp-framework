using Volo.Abp.Application.Dtos;

namespace MyStore.Inventory
{
    public class GetStockListDto : PagedAndSortedResultRequestDto
    {
        public string? ProductName { get; set; }
        public string? WarehouseName { get; set; }
    }
}
