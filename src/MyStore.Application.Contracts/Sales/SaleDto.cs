using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace MyStore.Sales
{
    public class SaleDto : EntityDto<Guid>
    {
        public string SalesNumber { get; set; }
        public DateTime SalesDate { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal OverallDiscount { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }
        public List<SaleItemDto> SaleItems { get; set; }
    }
}
