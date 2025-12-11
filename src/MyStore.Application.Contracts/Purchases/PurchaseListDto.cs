using System;
using Volo.Abp.Application.Dtos;

namespace MyStore.Purchases
{
    public class PurchaseListDto : EntityDto<Guid>
    {
        public string PurchaseNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SupplierName { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
    }
}
