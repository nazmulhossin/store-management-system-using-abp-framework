using Microsoft.AspNetCore.Authorization;
using MyStore.Inventory;
using MyStore.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using static Volo.Abp.UI.Navigation.DefaultMenuNames.Application;

namespace MyStore.Purchases
{
    [Authorize(MyStorePermissions.Purchases.Default)]
    public class PurchaseAppService : MyStoreAppService, IPurchaseAppService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly PurchaseManager _purchaseManager;
        private readonly StockManager _stockManager;

        public PurchaseAppService(
            IPurchaseRepository purchaseRepository, 
            PurchaseManager purchaseManager,
            StockManager stockManager)
        {
            _purchaseRepository = purchaseRepository;
            _purchaseManager = purchaseManager;
            _stockManager = stockManager;
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
                input.Filter
            );

            var totalCount = await _purchaseRepository.CountAsync();

            return new PagedResultDto<PurchaseListDto>(
                totalCount,
                ObjectMapper.Map<List<Purchase>, List<PurchaseListDto>>(purchases)
            );
        }

        [Authorize(MyStorePermissions.Purchases.Create)]
        public async Task<PurchaseDto> CreateAsync(CreateUpdatePurchaseDto input)
        {
            var purchase = await _purchaseManager.CreateAsync(
                input.PurchaseNumber,
                input.PurchaseDate,
                input.SupplierName,
                input.Description
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

                // update stock
                await _stockManager.AddStockAsync(
                    itemDto.ProductName,
                    itemDto.WarehouseName,
                    itemDto.Quantity
                );
            }

            purchase.SetPaidAmount(input.PaidAmount);
            purchase.EnsureHasPurchaseItems();

            await _purchaseRepository.InsertAsync(purchase);

            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase);
        }

        [Authorize(MyStorePermissions.Purchases.Edit)]
        public async Task<PurchaseDto> UpdateAsync(Guid id, CreateUpdatePurchaseDto input)
        {
            // Load the purchase aggregate including items
            var purchase = await _purchaseRepository.GetAsync(id, includeDetails: true);

            if (purchase == null)
            {
                throw new EntityNotFoundException(typeof(Purchase), id);
            }

            // Update header info
            purchase.UpdateHeaderInfo(
                input.PurchaseNumber,
                input.PurchaseDate,
                input.SupplierName,
                input.Description
            );

            var existingItemIds = purchase.PurchaseItems.Select(x => x.Id).ToHashSet();
            var inputItemIds = input.PurchaseItems
                .Where(x => x.Id.HasValue && x.Id.Value != Guid.Empty)
                .Select(x => x.Id!.Value)
                .ToHashSet();

            // Remove deleted items
            var itemsToRemove = existingItemIds.Except(inputItemIds).ToList();
            foreach (var itemId in itemsToRemove)
            {
                var item = purchase.PurchaseItems.First(x => x.Id == itemId);

                // Remove from stock
                await _stockManager.RemoveStockAsync(
                    item.ProductName,
                    item.WarehouseName,
                    item.Quantity
                );

                // Remove from purchase aggregate
                purchase.RemoveItem(itemId);
            }

            // Update existing items or add new items
            foreach (var itemDto in input.PurchaseItems)
            {
                if (itemDto.Id.HasValue && existingItemIds.Contains(itemDto.Id.Value))
                {
                    // Update existing item
                    var existingItem = purchase.PurchaseItems.First(x => x.Id == itemDto.Id.Value);
                    var quantityDifference = itemDto.Quantity - existingItem.Quantity;

                    purchase.UpdateItem(
                        itemDto.Id.Value,
                        itemDto.ProductName,
                        itemDto.WarehouseName,
                        itemDto.Quantity,
                        itemDto.UnitPrice,
                        itemDto.Discount
                    );

                    // Adjust stock via domain service
                    if (quantityDifference > 0)
                    {
                        await _stockManager.AddStockAsync(
                            itemDto.ProductName,
                            itemDto.WarehouseName,
                            quantityDifference
                        );
                    }
                    else if (quantityDifference < 0)
                    {
                        await _stockManager.RemoveStockAsync(
                            itemDto.ProductName,
                            itemDto.WarehouseName,
                            Math.Abs(quantityDifference)
                        );
                    }
                }
                else
                {
                    // Add new item
                    purchase.AddItem(
                        GuidGenerator.Create(),
                        itemDto.ProductName,
                        itemDto.WarehouseName,
                        itemDto.Quantity,
                        itemDto.UnitPrice,
                        itemDto.Discount
                    );

                    // Add stock via domain service
                    await _stockManager.AddStockAsync(
                        itemDto.ProductName,
                        itemDto.WarehouseName,
                        itemDto.Quantity
                    );
                }
            }

            // Set paid amount
            purchase.SetPaidAmount(input.PaidAmount);

            // Validate aggregate
            purchase.EnsureHasPurchaseItems();

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

            // Remove stock for each item
            foreach (var item in purchase.PurchaseItems)
            {
                await _stockManager.RemoveStockAsync(
                    item.ProductName,
                    item.WarehouseName,
                    item.Quantity
                );
            }

            // Delete the purchase (this also deletes the PurchaseItems via cascade)
            await _purchaseRepository.DeleteAsync(purchase);
        }
    }
}
