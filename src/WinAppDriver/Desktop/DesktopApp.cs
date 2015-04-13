namespace WinAppDriver.Desktop
{
    using System;
    using WinAppDriver.UAC;

    internal class DesktopApp : IDesktopApp
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IDriverContext context;

        private Capabilities capabilities;

        private IUACPomptHandler uacHandler;

        private IUtils utils;

        private IPackageInstaller installerCache;

        public DesktopApp(IDriverContext context, Capabilities capabilities, IUACPomptHandler uacHandler, IUtils utils)
        {
            this.context = context;
            this.capabilities = capabilities; // TODO validate capabilities
            this.uacHandler = uacHandler;
            this.utils = utils;
        }

        public string DriverAppID
        {
            get { return "MyDesktopApp"; } // TODO deduce the app ID
        }

        public Capabilities Capabilities
        {
            get { return this.capabilities; }
        }

        public IPackageInstaller Installer
        {
            get
            {
                if (this.installerCache == null)
                {
                    this.installerCache = new DesktopAppInstaller(this.context, this, this.uacHandler, this.utils);
                }

                return this.installerCache;
            }
        }

        public bool IsInstalled()
        {
            return false; // TODO with the help of external commands (exit status?)
        }

        public void Activate()
        {
            this.utils.Execute(this.capabilities.OpenCommand, null, false);
        }

        public void Terminate()
        {
            var command = this.capabilities.CloseCommand;
            if (command != null)
            {
                this.utils.Execute(command, null, false);
            }
        }

        public void BackupInitialStates()
        {
            throw new NotImplementedException();
        }

        public void RestoreInitialStates()
        {
            var command = this.capabilities.ResetCommand;
            if (command != null)
            {
                this.utils.Execute(command, null, false);
            }
        }

        public void Uninstall()
        {
            var command = this.capabilities.UninstallCommand;
            if (command != null)
            {
                this.utils.Execute(command, null, false);
            }
        }
    }
}