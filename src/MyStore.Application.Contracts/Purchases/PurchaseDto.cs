using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace MyStore.Purchases
{
    public class PurchaseDto : EntityDto<Guid>
    {
        public string PurchaseNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SupplierName { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal OverallDiscount { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }
        public List<PurchaseItemDto> PurchaseItems { get; set; } = new List<PurchaseItemDto>();
    }
}
