namespace WinAppDriver
{
    internal interface IApplication
    {
        string DriverAppID { get; }

        Capabilities Capabilities { get; }

        IPackageInstaller Installer { get; }

        bool IsInstalled();

        void Activate();

        void Terminate();

        void BackupInitialStates();

        void RestoreInitialStates();

        void Uninstall();
    }
}