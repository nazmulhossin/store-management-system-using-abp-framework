namespace MyStore.Permissions;

public static class MyStorePermissions
{
    public const string GroupName = "MyStore";


    public static class Books
    {
        public const string Default = GroupName + ".Books";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Purchases
    {
        public const string Default = GroupName + ".Purchases";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Inventory
    {
        public const string Default = GroupName + ".Inventory";
    }

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
}
