using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace MyStore.Sales
{
    public class Sale : FullAuditedAggregateRoot<Guid>
    {
        public string SalesNumber { get; private set; }
        public DateTime SalesDate { get; private set; }
        public string CustomerName { get; private set; }

        public decimal TotalAmount { get; private set; }
        public decimal OverallDiscount { get; private set; }
        public decimal PayableAmount { get; private set; }
        public decimal PaidAmount { get; private set; }
        public decimal DueAmount { get; private set; }

        public ICollection<SaleItem> SaleItems { get; private set; }

        private Sale() { }

        internal Sale(
            Guid id,
            string salesNumber,
            DateTime salesDate,
            string customerName,
            decimal paidAmount
        ) : base(id)
        {
            Validate(salesNumber, salesDate, customerName);

            SalesNumber = salesNumber;
            SalesDate = salesDate;
            CustomerName = customerName;
            PaidAmount = paidAmount;
            SaleItems = new List<SaleItem>();
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
            var item = new SaleItem(
                itemId,
                Id,
                productName,
                warehouseName,
                quantity,
                unitPrice,
                discount
            );

            SaleItems.Add(item);
            RecalculateTotals();
        }

        public void UpdateDetails(
            string salesNumber,
            DateTime salesDate,
            string customerName,
            decimal paidAmount)
        {
            Validate(salesNumber, salesDate, customerName);

            SalesNumber = salesNumber;
            SalesDate = salesDate;
            CustomerName = customerName;
            PaidAmount = paidAmount;
        }

        public void ClearItems()
        {
            if (!SaleItems.Any())
            {
                return;
            }

            SaleItems.Clear();
            RecalculateTotals();
        }

        public void EnsureHasSaleItems()
        {
            if (SaleItems == null || !SaleItems.Any())
                throw new BusinessException(MyStoreDomainErrorCodes.MustHaveAtLeastOneItem);
        }

        private void RecalculateTotals()
        {
            TotalAmount = SaleItems.Sum(x => x.SubTotal);
            PayableAmount = TotalAmount - OverallDiscount;
            DueAmount = PayableAmount - PaidAmount;
        }

        private void Validate(
            string salesNumber,
            DateTime salesDate,
            string customerName)
        {
            Check.NotNullOrWhiteSpace(salesNumber, nameof(salesNumber), maxLength: SaleConsts.MaxSaleNumberLength);
            Check.NotNullOrWhiteSpace(customerName, nameof(customerName), maxLength: SaleConsts.MaxCustomerNameLength);

            if (salesDate == default)
                throw new BusinessException(MyStoreDomainErrorCodes.InvalidSaleDate).WithData("SalesDate", salesDate);
        }
    }
}
