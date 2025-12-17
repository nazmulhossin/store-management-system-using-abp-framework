using Microsoft.AspNetCore.Authorization;
using MyStore.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace MyStore.Sales
{
    [Authorize(MyStorePermissions.Sales.Default)]
    public class SaleAppService : MyStoreAppService, ISaleAppService
    {
        private readonly SaleManager _saleManager;
        private readonly ISaleRepository _saleRepository;

        public SaleAppService(SaleManager saleManager, ISaleRepository saleRepository)
        {
            _saleManager = saleManager;
            _saleRepository = saleRepository;
        }

        public async Task<SaleDto> GetAsync(Guid id)
        {
            var sale = await _saleRepository.FindAsync(id, includeDetails: true);
            return ObjectMapper.Map<Sale, SaleDto>(sale);
        }

        public async Task<PagedResultDto<SaleListDto>> GetListAsync(GetSaleListDto input)
        {
            var sorting = input.Sorting.IsNullOrWhiteSpace() ? nameof(Sale.SalesDate) : input.Sorting;

            var sales = await _saleRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                sorting,
                input.CustomerName,
                input.StartDate,
                input.EndDate
            );

            var totalCount = await _saleRepository.GetCountAsync(
                input.CustomerName,
                input.StartDate,
                input.EndDate
            );

            return new PagedResultDto<SaleListDto>(
                totalCount,
                ObjectMapper.Map<List<Sale>, List<SaleListDto>>(sales)
            );
        }

        [Authorize(MyStorePermissions.Sales.Create)]
        public async Task<SaleDto> CreateAsync(CreateUpdateSaleDto input)
        {
            // Create sale
            var sale = await _saleManager.CreateAsync(
                input.SalesNumber,
                input.SalesDate,
                input.CustomerName,
                input.PaidAmount
            );

            // Add items
            foreach (var itemDto in input.SaleItems)
            {
                sale.AddItem(
                    GuidGenerator.Create(),
                    itemDto.ProductName,
                    itemDto.WarehouseName,
                    itemDto.Quantity,
                    itemDto.UnitPrice,
                    itemDto.Discount
                );
            }

            // Validate has at least one item
            sale.EnsureHasSaleItems();

            // Reduce stock
            await _saleManager.ReduceStockFromSaleAsync(sale);

            await _saleRepository.InsertAsync(sale);

            return ObjectMapper.Map<Sale, SaleDto>(sale);
        }

        [Authorize(MyStorePermissions.Sales.Edit)]
        public async Task<SaleDto> UpdateAsync(Guid id, CreateUpdateSaleDto input)
        {
            // Load existing sale including items
            var sale = await _saleRepository.FindAsync(id, includeDetails: true);
            if (sale == null)
            {
                throw new EntityNotFoundException(typeof(Sale), id);
            }

            // Restore stock from old sale
            await _saleManager.RestoreStockFromSaleAsync(sale);

            // Update sales details with new details
            sale.UpdateDetails(
                input.SalesNumber,
                input.SalesDate,
                input.CustomerName,
                input.PaidAmount
            );

            // Replace items (first delete all items then add new items)
            sale.ClearItems();
            foreach (var itemDto in input.SaleItems)
            {
                sale.AddItem(
                    GuidGenerator.Create(),
                    itemDto.ProductName,
                    itemDto.WarehouseName,
                    itemDto.Quantity,
                    itemDto.UnitPrice,
                    itemDto.Discount
                );
            }

            sale.EnsureHasSaleItems();

            // Reduce stock for new sale
            await _saleManager.ReduceStockFromSaleAsync(sale);

            await _saleRepository.UpdateAsync(sale);

            return ObjectMapper.Map<Sale, SaleDto>(sale);
        }

        [Authorize(MyStorePermissions.Sales.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            // Load the purchase with its items
            var sale = await _saleRepository.FindAsync(id, includeDetails: true);
            if (sale == null)
            {
                throw new EntityNotFoundException(typeof(Sale), id);
            }

            // Restore stock for all items
            await _saleManager.RestoreStockFromSaleAsync(sale);

            await _saleRepository.DeleteAsync(id);
        }
    }
}
