using Microsoft.AspNetCore.Authorization;
using MyStore.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace MyStore.Purchases
{
    [Authorize(MyStorePermissions.Purchases.Default)]
    public class PurchaseAppService : MyStoreAppService, IPurchaseAppService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly PurchaseManager _purchaseManager;

        public PurchaseAppService(IPurchaseRepository purchaseRepository, PurchaseManager purchaseManager)
        {
            _purchaseRepository = purchaseRepository;
            _purchaseManager = purchaseManager;
        }

        public async Task<PurchaseDto> GetAsync(Guid id)
        {
            var purchase = await _purchaseRepository.GetAsync(id);
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
            }

            purchase.SetPaidAmount(input.PaidAmount);

            await _purchaseRepository.InsertAsync(purchase);

            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase);
        }

        [Authorize(MyStorePermissions.Purchases.Edit)]
        public async Task<PurchaseDto> UpdateAsync(Guid id, CreateUpdatePurchaseDto input)
        {
            var purchase = await _purchaseRepository.GetAsync(id, includeDetails: true);

            purchase.UpdateHeaderInfo(
                input.PurchaseNumber,
                input.PurchaseDate,
                input.SupplierName,
                input.Description
            );

            // remove existing items; It aslo delete PurchaseItems from db when save
            purchase.PurchaseItems.Clear();

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

            purchase.SetPaidAmount(input.PaidAmount);

            await _purchaseRepository.UpdateAsync(purchase);

            return ObjectMapper.Map<Purchase, PurchaseDto>(purchase);
        }

        [Authorize(MyStorePermissions.Purchases.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            await _purchaseRepository.DeleteAsync(id);
        }
    }
}
