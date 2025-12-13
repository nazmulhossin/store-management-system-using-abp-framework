using System;
using Volo.Abp;

namespace MyStore.Purchases
{
    public class InvalidPurchaseDateException : BusinessException
    {
        public InvalidPurchaseDateException(DateTime purchaseDate)
        : base(MyStoreDomainErrorCodes.InvalidPurchaseDate)
        {
            WithData("PurchaseDate", purchaseDate);
        }
    }
}
