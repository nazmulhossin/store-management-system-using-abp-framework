using MyStore.Inventory;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace MyStore.Sales
{
    public class SaleManager : DomainService
    {
        private readonly StockManager _stockManager;

        public SaleManager(StockManager stockManager)
        {
            _stockManager = stockManager;
        }

        public async Task<Sale> CreateAsync(
            string salesNumber,
            DateTime salesDate,
            string customerName,
            decimal paidAmount = 0)
        {
            return new Sale(
                GuidGenerator.Create(),
                salesNumber,
                salesDate,
                customerName,
                paidAmount
            );
        }

        public async Task ReduceStockFromSaleAsync(Sale sale)
        {
            foreach (var item in sale.SaleItems)
            {
                await _stockManager.DecreaseStockAsync(
                    item.ProductName,
                    item.WarehouseName,
                    item.Quantity
                );
            }
        }

        public async Task RestoreStockFromSaleAsync(Sale sale)
        {
            foreach (var item in sale.SaleItems)
            {
                await _stockManager.IncreaseStockAsync(
                    item.ProductName,
                    item.WarehouseName,
                    item.Quantity
                );
            }
        }
    }
}
