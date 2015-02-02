namespace WinAppDriver
{
    using System;

    internal interface IPackageInstaller
    {
        bool IsBuildChanged();

        void Install();
    }
}
