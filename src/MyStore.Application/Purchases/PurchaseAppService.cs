using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using static MyStore.Permissions.MyStorePermissions;

namespace MyStore.Purchases
{
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

        public async Task<PurchaseDto> CreateAsync(CreateUpdatePurchaseDto input)
        {
            var purchase = await _purchaseManager.CreateAsync(
                input.purchaseNumber,
                input.PurchaseDate,
                input.SupplierName,
                input.Description
            );

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
    }
}
