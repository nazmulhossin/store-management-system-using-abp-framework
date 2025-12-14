using System;
using Volo.Abp.Application.Dtos;

namespace MyStore.Sales
{
    public class SaleListDto : EntityDto<Guid>
    {
        public string SalesNumber { get; set; }
        public DateTime SalesDate { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
    }
}
