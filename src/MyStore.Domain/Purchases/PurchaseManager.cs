using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace MyStore.Purchases
{
    public class PurchaseManager : DomainService
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public PurchaseManager(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public async Task<Purchase> CreateAsync(
            string purchaseNumber,
            DateTime purchaseDate,
            string supplierName,
            string description)
        {
            return new Purchase(
                GuidGenerator.Create(),
                purchaseNumber,
                purchaseDate,
                supplierName,
                description
            );
        }
    }
}
