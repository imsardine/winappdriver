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
            get { return this.capabilities.AppID ?? "Windows"; }
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
            var command = this.capabilities.CheckInstalledCommand;
            if (command == null)
            {
                return true;
            }

            int exitCode;
            this.utils.Execute(command, null, out exitCode);

            return exitCode == 0;
        }

        public void Activate()
        {
            var command = this.capabilities.OpenCommand;
            if (command != null)
            {
                this.utils.Execute(command, null);
            }
        }

        public void Terminate()
        {
            var command = this.capabilities.CloseCommand;
            if (command != null)
            {
                int exitCode;
                this.utils.Execute(command, null, out exitCode);
            }
        }

        public void BackupInitialStates()
        {
            var command = this.capabilities.BackupStatesCommand;
            if (command != null)
            {
                int exitCode;
                this.utils.Execute(command, null, out exitCode);
            }
        }

        public void RestoreInitialStates()
        {
            var command = this.capabilities.RestoreStatesCommand;
            if (command != null)
            {
                int exitCode;
                this.utils.Execute(command, null, out exitCode);
            }
        }

        public void Uninstall()
        {
            var command = this.capabilities.UninstallCommand;
            if (command != null)
            {
                try
                {
                    this.uacHandler.Activate(true);

                    int exitCode;
                    this.utils.Execute(command, null, out exitCode);
                }
                finally
                {
                    this.uacHandler.Deactivate();
                }
            }
        }
    }
}