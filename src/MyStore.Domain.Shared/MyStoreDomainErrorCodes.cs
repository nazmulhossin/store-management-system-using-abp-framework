namespace MyStore;

public static class MyStoreDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */
    public const string InvalidPurchaseDate = "StoreApp:00001";
    public const string MustHaveAtLeastOneItem = "StoreApp:00002";
    public const string QuantityMustBeGreaterThanZero = "StoreApp:00003";
    public const string ItemQuantityExceedsAvailableStock = "StoreApp:00004";
    public const string StockNotFoundForProductAndWarehouse = "StoreApp:00005";
    public const string PurchaseItemUnitPriceMustBeGreaterThanZero = "StoreApp:00006";
    public const string PurchaseItemDiscountCanNotBeNegative = "StoreApp:00007";

    public const string InvalidSaleDate = "StoreApp:00008";
    public const string SaleItemNotFound = "StoreApp:00009";
}
