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
            string? description = null,
            decimal paidAmount = 0
        ) : base(id)
        {
            Validate(purchaseNumber, purchaseDate, supplierName);

            PurchaseNumber = purchaseNumber;
            PurchaseDate = purchaseDate;
            SupplierName = supplierName;
            Description = description;
            PaidAmount = paidAmount;
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

        public void EnsureHasPurchaseItems()
        {
            if (PurchaseItems == null || !PurchaseItems.Any())
                throw new BusinessException(MyStoreDomainErrorCodes.MustHaveAtLeastOneItem);
        }

        public void UpdateDetails(
            string purchaseNumber,
            DateTime purchaseDate,
            string supplierName,
            string description,
            decimal paidAmount)
        {
            Validate(purchaseNumber, purchaseDate, supplierName);

            PurchaseNumber = purchaseNumber;
            PurchaseDate = purchaseDate;
            SupplierName = supplierName;
            Description = description;
            PaidAmount = paidAmount;
        }

        public void ClearItems()
        {
            if (!PurchaseItems.Any())
            {
                return;
            }

            PurchaseItems.Clear();
            RecalculateTotals();
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
