using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace MyStore.Purchases
{
    public class Purchase : FullAuditedAggregateRoot<Guid>
    {
        public string PurchaseNumber { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public string SupplierName { get; private set; }
        public string Description { get; private set; }

        public decimal TotalAmount { get; private set; }
        public decimal OverallDiscount { get; private set; }
        public decimal PayableAmount { get; private set; }
        public decimal PaidAmount { get; private set; }
        public decimal DueAmount { get; private set; }

        public ICollection<PurchaseItem> PurchaseItems { get; private set; }

        private Purchase() { }

        internal Purchase(
            Guid id,
            string purchaseNumber,
            DateTime purchaseDate,
            string supplierName,
            string? description = null
        ) : base(id)
        {
            Validate(purchaseNumber, purchaseDate, supplierName);

            PurchaseNumber = purchaseNumber;
            PurchaseDate = purchaseDate;
            SupplierName = supplierName;
            Description = description;
            PurchaseItems = new List<PurchaseItem>();
        }

        public void AddItem(
            Guid itemId,
            string productName,
            string warehouseName,
            int quantity,
            decimal unitPrice,
            decimal discount = 0
        )
        {
            var item = new PurchaseItem(
                itemId,
                Id,
                productName,
                warehouseName,
                quantity,
                unitPrice,
                discount
            );
            PurchaseItems.Add(item);
            RecalculateTotals();
        }

        public void SetPaidAmount(decimal paidAmount)
        {
            PaidAmount = paidAmount;
            RecalculateTotals();
        }

        public void UpdateHeaderInfo(
            string purchaseNumber,
            DateTime purchaseDate,
            string supplierName,
            string description)
        {
            Validate(purchaseNumber, purchaseDate, supplierName);

            PurchaseNumber = purchaseNumber;
            PurchaseDate = purchaseDate;
            SupplierName = supplierName;
            Description = description;
        }

        public void RemoveItem(Guid itemId)
        {
            PurchaseItems.RemoveAll(x => x.Id == itemId);
            RecalculateTotals();
        }

        public void UpdateItem(
            Guid itemId,
            string productCode,
            string warehouseCode,
            int quantity,
            decimal unitPrice,
            decimal discount
        )
        {
            var item = PurchaseItems.FirstOrDefault(x => x.Id == itemId);
            if (item != null)
            {
                item.UpdateDetails(productCode, warehouseCode, quantity, unitPrice, discount);
                RecalculateTotals();
            }
        }

        private void RecalculateTotals()
        {
            TotalAmount = PurchaseItems.Sum(x => x.SubTotal);
            PayableAmount = TotalAmount - OverallDiscount;
            DueAmount = PayableAmount - PaidAmount;
        }

        private void Validate(
            string purchaseNumber,
            DateTime purchaseDate,
            string supplierName)
        {
            Check.NotNullOrWhiteSpace(purchaseNumber, nameof(purchaseNumber), maxLength: PurchaseConsts.MaxPurchaseNumberLength);
            Check.NotNullOrWhiteSpace(supplierName, nameof(supplierName), maxLength: PurchaseConsts.MaxSupplierNameLength);

            if (purchaseDate == default)
                throw new InvalidPurchaseDateException(purchaseDate);
        }
    }
}
