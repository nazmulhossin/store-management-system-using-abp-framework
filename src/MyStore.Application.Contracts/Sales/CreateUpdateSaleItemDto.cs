using System;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Sales
{
    public class CreateUpdateSaleItemDto
    {
        public Guid? Id { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string WarehouseName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; }
    }
}
