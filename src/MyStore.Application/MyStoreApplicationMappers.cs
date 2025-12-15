using MyStore.Inventory;
using MyStore.Purchases;
using MyStore.Sales;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace MyStore;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PurchaseToPurchaseDtoMapper : MapperBase<Purchase, PurchaseDto>
{
    public override partial PurchaseDto Map(Purchase source);
    public override partial void Map(Purchase source, PurchaseDto destination);

    // Define the item mapper
    private partial PurchaseItemDto MapItem(PurchaseItem source);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PurchaseToPurchaseListDtoMapper : MapperBase<Purchase, PurchaseListDto>
{
    public override partial PurchaseListDto Map(Purchase source);

    public override partial void Map(Purchase source, PurchaseListDto destination);
}


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StockToStockDtoMapper : MapperBase<Stock, StockDto>
{
    public override partial StockDto Map(Stock source);

    public override partial void Map(Stock source, StockDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SaleToSaleDtoMapper : MapperBase<Sale, SaleDto>
{
    public override partial SaleDto Map(Sale source);
    public override partial void Map(Sale source, SaleDto destination);

    // Define the item mapper
    private partial SaleItemDto MapItem(SaleItem source);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SaleToSaleListDtoMapper : MapperBase<Sale, SaleListDto>
{
    public override partial SaleListDto Map(Sale source);
    public override partial void Map(Sale source, SaleListDto destination);
}
