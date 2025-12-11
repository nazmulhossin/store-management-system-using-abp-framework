using MyStore.Books;
using Xunit;

namespace MyStore.EntityFrameworkCore.Applications.Books;

[Collection(MyStoreTestConsts.CollectionDefinitionName)]
public class EfCoreBookAppService_Tests : BookAppService_Tests<MyStoreEntityFrameworkCoreTestModule>
{

}