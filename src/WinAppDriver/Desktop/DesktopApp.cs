namespace WinAppDriver.Desktop
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
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
            get { return this.capabilities.AppID != null ? this.capabilities.AppID + " (Desktop)" : "Windows (Desktop)"; }
        }

        public Capabilities Capabilities
        {
            get { return this.capabilities; }
        }

        public string StatesDir
        {
            get { return Path.Combine(this.context.GetAppWorkingDir(this), "States"); }
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
            this.TriggerCustomAction(command, null, out exitCode);

            return exitCode == 0;
        }

        public void Activate()
        {
            var command = this.capabilities.OpenCommand;
            if (command != null)
            {
                this.TriggerCustomAction(command, null);
            }
        }

        public void Terminate()
        {
            var command = this.capabilities.CloseCommand;
            if (command != null)
            {
                int exitCode;
                this.TriggerCustomAction(command, null, out exitCode);
            }
        }

        public bool BackupInitialStates(bool overwrite)
        {
            var command = this.capabilities.BackupStatesCommand;
            if (command == null || (this.utils.DirectoryExists(this.StatesDir) && !overwrite))
            {
                return false;
            }

            var envs = new Dictionary<string, string>();
            this.PrepareStatesDir(envs, overwrite);

            int exitCode; // TODO throw exception and delete the directory
            this.TriggerCustomAction(command, envs, out exitCode);
            return true;
        }

        public void RestoreInitialStates()
        {
            var command = this.capabilities.RestoreStatesCommand;
            if (command == null || !this.utils.DirectoryExists(this.StatesDir))
            {
                return;
            }

            var envs = new Dictionary<string, string>();
            this.PrepareStatesDir(envs, false);

            int exitCode; // TODO throw exception and delete the directory
            this.TriggerCustomAction(command, envs, out exitCode);
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
                    this.TriggerCustomAction(command, null, out exitCode);
                }
                finally
                {
                    this.uacHandler.Deactivate();
                }
            }
        }

        public Process TriggerCustomAction(string command, IDictionary<string, string> envs)
        {
            envs = envs ?? new Dictionary<string, string>();
            return this.utils.Execute(command, envs);
        }

        public Process TriggerCustomAction(string command, IDictionary<string, string> envs, out int waitExitCode)
        {
            var process = this.TriggerCustomAction(command, envs);
            process.WaitForExit(3 * 60 * 1000); // 3 minutes

            waitExitCode = process.ExitCode;
            return process;
        }

        private void PrepareStatesDir(IDictionary<string, string> envs, bool overwrite)
        {
            var dir = this.StatesDir;

            if (overwrite)
            {
                this.utils.DeleteDirectoryIfExists(dir);
            }

            this.utils.CreateDirectoryIfNotExists(dir);
            envs.Add("WINAPPDRIVER_STATES_DIR", dir);
        }
    }
}