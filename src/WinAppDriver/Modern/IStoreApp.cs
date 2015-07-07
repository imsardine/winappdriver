namespace WinAppDriver.Modern
{
    internal interface IStoreApp : IApplication
    {
        string PackageName { get; }

        string AppUserModelId { get; }

        string PackageFamilyName { get; }

        string PackageFullName { get; }

        string PackageFolderDir { get; }
    }
}