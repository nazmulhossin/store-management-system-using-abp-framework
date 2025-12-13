using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace MyStore.Inventory
{
    public class StockManager : DomainService
    {
        private readonly IStockRepository _stockRepository;

        public StockManager(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task AddStockAsync(string productName, string warehouseName, int quantity)
        {
            if (quantity <= 0)
            {
                throw new BusinessException(MyStoreDomainErrorCodes.QuantityMustBeGreaterThanZero);
            }

            var stock = await _stockRepository.FindByProductAndWarehouseAsync(productName, warehouseName);
            if (stock == null)
            {
                stock = new Stock(GuidGenerator.Create(), productName, warehouseName, quantity);
                await _stockRepository.InsertAsync(stock);
            }
            else
            {
                stock.AddStock(quantity);
                await _stockRepository.UpdateAsync(stock);
            }
        }

        public async Task RemoveStockAsync(string productName, string warehouseName, int quantity)
        {
            if (quantity <= 0)
            {
                throw new BusinessException(MyStoreDomainErrorCodes.QuantityMustBeGreaterThanZero);
            }

            var stock = await _stockRepository.FindByProductAndWarehouseAsync(productName, warehouseName);
            if (stock == null)
            {
                throw new BusinessException(MyStoreDomainErrorCodes.StockNotFoundForProductAndWarehouse)
                    .WithData("ProductName", productName)
                    .WithData("WarehouseName", warehouseName);
            }

            stock.RemoveStock(quantity);
            await _stockRepository.UpdateAsync(stock);
        }
    }
}
