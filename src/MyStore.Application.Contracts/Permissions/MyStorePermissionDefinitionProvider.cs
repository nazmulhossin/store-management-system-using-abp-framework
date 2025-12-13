using MyStore.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace MyStore.Permissions;

public class MyStorePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MyStorePermissions.GroupName);

        var booksPermission = myGroup.AddPermission(MyStorePermissions.Books.Default, L("Permission:Books"));
        booksPermission.AddChild(MyStorePermissions.Books.Create, L("Permission:Books.Create"));
        booksPermission.AddChild(MyStorePermissions.Books.Edit, L("Permission:Books.Edit"));
        booksPermission.AddChild(MyStorePermissions.Books.Delete, L("Permission:Books.Delete"));

        var purchasesPermission = myGroup.AddPermission(MyStorePermissions.Purchases.Default, L("Permission:Purchases"));
        purchasesPermission.AddChild(MyStorePermissions.Purchases.Create, L("Permission:Purchases.Create"));
        purchasesPermission.AddChild(MyStorePermissions.Purchases.Edit, L("Permission:Purchases.Edit"));
        purchasesPermission.AddChild(MyStorePermissions.Purchases.Delete, L("Permission:Purchases.Delete"));

        var inventoryPermission = myGroup.AddPermission(MyStorePermissions.Inventory.Default, L("Permission:Inventory"));

        //Define your own permissions here. Example:
        //myGroup.AddPermission(MyStorePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MyStoreResource>(name);
    }
}
