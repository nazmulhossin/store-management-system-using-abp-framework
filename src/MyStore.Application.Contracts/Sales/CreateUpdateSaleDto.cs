using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Sales
{
    public class CreateUpdateSaleDto
    {
        [Required]
        public string SalesNumber { get; set; }

        [Required]
        public DateTime SalesDate { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PaidAmount { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateUpdateSaleItemDto> SaleItems { get; set; } = new List<CreateUpdateSaleItemDto>();
    }
}
