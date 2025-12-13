using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Purchases
{
    public class CreateUpdatePurchaseDto
    {
        [Required]
        public string PurchaseNumber { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public string SupplierName { get; set; }

        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal PaidAmount { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateUpdatePurchaseItemDto> PurchaseItems { get; set; } = new List<CreateUpdatePurchaseItemDto>();
    }
}
