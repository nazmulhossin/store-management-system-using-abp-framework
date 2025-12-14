using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace MyStore.Inventory
{
    public class StockManager : DomainService
    {
        private readonly IStockRepository _stockRepository;

        public StockManager(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task IncreaseOrCreateStockAsync(string productName, string warehouseName, int quantity)
        {
            if (quantity <= 0)
                throw new BusinessException(MyStoreDomainErrorCodes.QuantityMustBeGreaterThanZero);

            var stock = await _stockRepository.FindByProductAndWarehouseAsync(productName, warehouseName);
            if (stock == null)
            {
                stock = new Stock(GuidGenerator.Create(), productName, warehouseName, quantity);
                await _stockRepository.InsertAsync(stock);
            }
            else
            {
                stock.IncreaseStock(quantity);
                await _stockRepository.UpdateAsync(stock);
            }
        }

        public async Task IncreaseStockAsync(string productName, string warehouseName, int quantity)
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

            stock.IncreaseStock(quantity);
            await _stockRepository.UpdateAsync(stock);
        }

        public async Task DecreaseStockAsync(string productName, string warehouseName, int quantity)
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

            stock.DecreaseStock(quantity);
            await _stockRepository.UpdateAsync(stock);
        }
    }
}
