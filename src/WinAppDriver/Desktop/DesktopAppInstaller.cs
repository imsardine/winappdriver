namespace WinAppDriver.Desktop
{
    using System.Collections.Generic;
    using WinAppDriver.UAC;

    internal class DesktopAppInstaller : AppInstaller<IDesktopApp>
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IDesktopApp app;

        private IUACPomptHandler uacHandler;

        public DesktopAppInstaller(IDriverContext context, IDesktopApp app, IUACPomptHandler uacHandler, IUtils utils)
            : base(context, app, utils)
        {
            this.app = app;
            this.uacHandler = uacHandler;
        }

        protected internal override void InstallImpl(string package, string checksum)
        {
            // Directly execute the installation package, if the external command is not provided.
            var caps = this.app.Capabilities;
            string command;
            if (caps.InstallCommand != null)
            {
                command = caps.InstallCommand;
                logger.Info(
                    "Invoke the install command '{0}' to carry out the installation of package '{1}'.",
                    command, package);
            }
            else
            {
                command = package;
                logger.Info(
                    "Install command is not provided, so directly invoke the (executable?) installation package '{0}'.",
                    package);
            }

            var envs = new Dictionary<string, string>
            {
                { "WinAppDriverInstallationPackage", package }
            };

            try
            {
                this.uacHandler.Activate(true);

                int exitCode;
                this.app.TriggerCustomAction(command, envs, out exitCode);
            }
            finally
            {
                this.uacHandler.Deactivate();
            }
        }
    }
}