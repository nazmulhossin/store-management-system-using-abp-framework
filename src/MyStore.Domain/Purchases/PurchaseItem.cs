using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace MyStore.Purchases
{
    public class PurchaseItem : Entity<Guid>
    {
        public Guid PurchaseId { get; private set; }
        public string ProductName { get; private set; }
        public string WarehouseName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal SubTotal { get; private set; }
        public decimal Discount { get; private set; }

        private PurchaseItem() { }

        public PurchaseItem(
            Guid id,
            Guid purchaseId,
            string productName,
            string warehouseName,
            int quantity,
            decimal unitPrice,
            decimal discount = 0
        ) : base(id)
        {
            PurchaseId = purchaseId;
            ValidateItem(productName, warehouseName, quantity, unitPrice, discount);
            Quantity = quantity;
            UnitPrice = unitPrice;
            Discount = discount;
            CalculateSubTotal();
        }

        public void UpdateDetails(
            string productName,
            string warehouseName,
            int quantity,
            decimal unitPrice,
            decimal discount
        )
        {
            ValidateItem(productName, warehouseName, quantity, unitPrice, discount);
            Quantity = quantity;
            UnitPrice = unitPrice;
            Discount = discount;
            CalculateSubTotal();
        }

        private void CalculateSubTotal()
        {
            SubTotal = (Quantity * UnitPrice) - Discount;
        }

        private void ValidateItem(
            string productName,
            string warehouseName,
            int quantity,
            decimal unitPrice,
            decimal discount)
        {
            ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName), maxLength: PurchaseConsts.MaxProductNameLength);
            WarehouseName = Check.NotNullOrWhiteSpace(warehouseName, nameof(warehouseName), maxLength: PurchaseConsts.MaxWarehouseNameLength);

            if (quantity <= 0)
                throw new BusinessException(MyStoreDomainErrorCodes.QuantityMustBeGreaterThanZero);

            if (unitPrice <= 0)
                throw new BusinessException(MyStoreDomainErrorCodes.PurchaseItemUnitPriceMustBeGreaterThanZero);

            if (discount < 0)
                throw new BusinessException(MyStoreDomainErrorCodes.PurchaseItemDiscountCanNotBeNegative);
        }
    }
}
