using MyStore.Inventory;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace MyStore.Purchases
{
    public class PurchaseManager : DomainService
    {
        private readonly StockManager _stockManager;

        public PurchaseManager(StockManager stockManager)
        {
            _stockManager = stockManager;
        }

        public async Task<Purchase> CreateAsync(
            string purchaseNumber,
            DateTime purchaseDate,
            string supplierName,
            string? description = null,
            decimal paidAmount = 0
            )
        {
            return new Purchase(
                GuidGenerator.Create(),
                purchaseNumber,
                purchaseDate,
                supplierName,
                description,
                paidAmount
            );
        }

        public async Task IncreaseStockFromPurchaseAsync(Purchase purchase)
        {
            foreach (var item in purchase.PurchaseItems)
            {
                await _stockManager.IncreaseOrCreateStockAsync(
                    item.ProductName,
                    item.WarehouseName,
                    item.Quantity
                );
            }
        }

        public async Task ReduceStockFromPurchaseAsync(Purchase purchase)
        {
            foreach (var item in purchase.PurchaseItems)
            {
                await _stockManager.DecreaseStockAsync(
                    item.ProductName,
                    item.WarehouseName,
                    item.Quantity
                );
            }
        }
    }
}
