using System;
using Volo.Abp.Application.Dtos;

namespace MyStore.Purchases
{
    public class PurchaseItemDto : EntityDto<Guid>
    {
        public string ProductName { get; set; }
        public string WarehouseName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
    }
}
