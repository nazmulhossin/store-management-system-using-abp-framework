using Microsoft.AspNetCore.Authorization;
using MyStore.Inventory;
using MyStore.Permissions;
using MyStore.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace MyStore.Purchases
{
    [Authorize(MyStorePermissions.Purchases.Default)]
    public class PurchaseAppService : MyStoreAppService, IPurchaseAppService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly PurchaseManager _purchaseManager;

        public PurchaseAppService(
            IPurchaseRepository purchaseRepository, 
            PurchaseManager purchaseManager,
            StockManager stockManager)
        {
            _purchaseRepository = purchaseRepository;
            _purchaseManager = purchaseManager;
        }

        public async Task<PurchaseDto> GetAsync(Guid id)
        {
            var purchase = await _purchaseRepository.GetAsync(id, includeDetails: true);
            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase);
        }

        public async Task<PagedResultDto<PurchaseListDto>> GetListAsync(GetPurchaseListDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(Purchase.PurchaseDate);
            }

            var purchases = await _purchaseRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.SupplierName,
                input.StartDate,
                input.EndDate
            );

            var totalCount = await _purchaseRepository.GetCountAsync(
                input.SupplierName,
                input.StartDate,
                input.EndDate
            );

            return new PagedResultDto<PurchaseListDto>(
                totalCount,
                ObjectMapper.Map<List<Purchase>, List<PurchaseListDto>>(purchases)
            );
        }

        [Authorize(MyStorePermissions.Purchases.Create)]
        public async Task<PurchaseDto> CreateAsync(CreateUpdatePurchaseDto input)
        {
            // Create a new purchase
            var purchase = await _purchaseManager.CreateAsync(
                input.PurchaseNumber,
                input.PurchaseDate,
                input.SupplierName,
                input.Description,
                input.PaidAmount
            );

            // Add items
            foreach (var itemDto in input.PurchaseItems)
            {
                purchase.AddItem(
                    GuidGenerator.Create(),
                    itemDto.ProductName,
                    itemDto.WarehouseName,
                    itemDto.Quantity,
                    itemDto.UnitPrice,
                    itemDto.Discount
                );
            }

            purchase.EnsureHasPurchaseItems();

            // Increase stock for new purchase
            await _purchaseManager.IncreaseStockFromPurchaseAsync(purchase);

            await _purchaseRepository.InsertAsync(purchase);

            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase);
        }

        [Authorize(MyStorePermissions.Purchases.Edit)]
        public async Task<PurchaseDto> UpdateAsync(Guid id, CreateUpdatePurchaseDto input)
        {
            // Load the existing purchase including items
            var purchase = await _purchaseRepository.GetAsync(id, includeDetails: true);
            if (purchase == null)
            {
                throw new EntityNotFoundException(typeof(Purchase), id);
            }

            // Reduce stock for old purchase
            await _purchaseManager.ReduceStockFromPurchaseAsync(purchase);

            // Update purchase details with new details
            purchase.UpdateDetails(
                input.PurchaseNumber,
                input.PurchaseDate,
                input.SupplierName,
                input.Description,
                input.PaidAmount
            );

            // Replace items (first delete all items then add new items)
            purchase.ClearItems();
            foreach (var itemDto in input.PurchaseItems)
            {
                purchase.AddItem(
                    GuidGenerator.Create(),
                    itemDto.ProductName,
                    itemDto.WarehouseName,
                    itemDto.Quantity,
                    itemDto.UnitPrice,
                    itemDto.Discount
                );
            }

            // Validate aggregate
            purchase.EnsureHasPurchaseItems();

            // Increase stock for purchase
            await _purchaseManager.IncreaseStockFromPurchaseAsync(purchase);

            await _purchaseRepository.UpdateAsync(purchase);

            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase);
        }

        [Authorize(MyStorePermissions.Purchases.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            // Load the purchase with its items
            var purchase = await _purchaseRepository.GetAsync(id, includeDetails: true);
            if (purchase == null)
            {
                throw new EntityNotFoundException(typeof(Purchase), id);
            }

            // Reduce stock for each item
            await _purchaseManager.ReduceStockFromPurchaseAsync(purchase);

            // Delete the purchase (this also deletes the PurchaseItems via cascade)
            await _purchaseRepository.DeleteAsync(purchase);
        }
    }
}
