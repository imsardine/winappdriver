namespace WinAppDriver.Desktop
{
    using WinAppDriver.UAC;

    internal class DesktopAppInstaller : IPackageInstaller
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IDesktopApp app;

        private IUACPomptHandler uacHandler;

        private IUtils utils;

        public DesktopAppInstaller(IDesktopApp app, IUACPomptHandler uacHandler, IUtils utils)
        {
            this.app = app;
            this.uacHandler = uacHandler;
            this.utils = utils;
        }

        public bool IsBuildChanged()
        {
            return false; // TODO
        }

        public void Install()
        {
            // TODO download installer...
            try
            {
                this.uacHandler.Activate(true);
                this.utils.Execute(this.app.Capabilities.InstallCommand, "Install", true);
            }
            finally
            {
                this.uacHandler.Deactivate();
            }
        }
    }
}