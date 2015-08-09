namespace WinAppDriver
{
    internal interface IApplication
    {
        string DriverAppID { get; }

        Capabilities Capabilities { get; }

        string StatesDir { get; }

        IPackageInstaller Installer { get; }

        bool IsInstalled();

        void Activate();

        void Terminate();

        bool BackupInitialStates(bool overwrite);

        void RestoreInitialStates();

        void Uninstall();
    }
}