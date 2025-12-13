using MyStore.Books;
using MyStore.Inventory;
using MyStore.Purchases;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace MyStore;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MyStoreBookToBookDtoMapper : MapperBase<Book, BookDto>
{
    public override partial BookDto Map(Book source);

    public override partial void Map(Book source, BookDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MyStoreCreateUpdateBookDtoToBookMapper : MapperBase<CreateUpdateBookDto, Book>
{
    public override partial Book Map(CreateUpdateBookDto source);

    public override partial void Map(CreateUpdateBookDto source, Book destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PurchaseMapper : MapperBase<Purchase, PurchaseDto>
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
