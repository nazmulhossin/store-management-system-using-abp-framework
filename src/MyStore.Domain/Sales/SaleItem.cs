using System;
using Volo.Abp.Domain.Entities;

namespace MyStore.Sales
{
    public class SaleItem : Entity<Guid>
    {
        public Guid SaleId { get; private set; }
        public string ProductName { get; private set; }
        public string WarehouseName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal SubTotal { get; private set; }
        public decimal Discount { get; private set; }

        private SaleItem() { }

        internal SaleItem(
            Guid id,
            Guid saleId,
            string productName,
            string warehouseName,
            int quantity,
            decimal unitPrice,
            decimal discount = 0
        ) : base(id)
        {
            SaleId = saleId;
            ProductName = productName;
            WarehouseName = warehouseName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Discount = discount;
            CalculateSubTotal();
        }

        public void UpdateItem(
            string productName,
            string warehouseName,
            int quantity,
            decimal unitPrice,
            decimal discount
        )
        {
            ProductName = productName;
            WarehouseName = warehouseName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Discount = discount;
            CalculateSubTotal();
        }

        private void CalculateSubTotal()
        {
            SubTotal = (Quantity * UnitPrice) - Discount;
        }
    }
}
