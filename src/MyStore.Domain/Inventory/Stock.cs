using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace MyStore.Inventory
{
    public class Stock : FullAuditedAggregateRoot<Guid>
    {
        public string ProductName { get; set; }
        public string WarehouseName { get; set; }
        public int CurrentStock { get; private set; }

        private Stock() { }

        internal Stock(Guid id, string productName, string warehouseName, int initialQuantity) : base(id)
        {
            ProductName = productName;
            WarehouseName = warehouseName;
            CurrentStock = initialQuantity > 0 ? initialQuantity : throw new BusinessException(MyStoreDomainErrorCodes.QuantityMustBeGreaterThanZero);
        }

        internal void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new BusinessException(MyStoreDomainErrorCodes.QuantityMustBeGreaterThanZero);

            CurrentStock += quantity;
        }

        internal void DecreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new BusinessException(MyStoreDomainErrorCodes.QuantityMustBeGreaterThanZero);

            if (CurrentStock < quantity)
                throw new BusinessException(MyStoreDomainErrorCodes.ItemQuantityExceedsAvailableStock);

            CurrentStock -= quantity;
        }
    }
}
