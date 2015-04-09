namespace WinAppDriver.Desktop
{
    using System.IO;
    using System.Text;
    using WinAppDriver.UAC;

    internal class DesktopAppInstaller : IPackageInstaller
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IDriverContext context;

        private IDesktopApp app;

        private IUACPomptHandler uacHandler;

        private IUtils utils;

        private bool prepared = false;

        public DesktopAppInstaller(IDriverContext context, IDesktopApp app, IUACPomptHandler uacHandler, IUtils utils)
        {
            this.context = context;
            this.app = app;
            this.uacHandler = uacHandler;
            this.utils = utils;
        }

        public bool IsBuildChanged()
        {
            string currentMD5;
            if (!this.TryReadCurrent(out currentMD5))
            {
                logger.Info(
                    "Build changed, because current build (MD5) is unknown; app = [{0}]",
                    this.app.DriverAppID);
                return true;
            }

            string packageMD5;
            string package = this.PrepareInstallationPackage(out packageMD5);
            bool changed = currentMD5 != packageMD5;

            logger.Info(
                "Build changed? {0}; app = [{1}], current MD5 = [{2}], package MD5 = [{3}]",
                changed, this.app.DriverAppID, currentMD5, packageMD5);
            return changed;
        }

        public void Install()
        {
            string md5;
            string instPackage = this.PrepareInstallationPackage(out md5);
            // TODO Pass the package through environment variable AppInstallationPackage

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

        private string CurrentFile
        {
            get
            {
                return Path.Combine(this.context.GetAppWorkingDir(this.app), "Current.txt");
            }
        }

        private string PackageDir
        {
            get
            {
                return Path.Combine(this.context.GetAppWorkingDir(this.app), "InstallationPackage");
            }
        }

        private string PackageInfoFile
        {
            get
            {
                return Path.Combine(this.PackageDir, "Info.txt");
            }
        }

        private bool TryReadCurrent(out string md5)
        {
            string path = this.CurrentFile;
            logger.Debug("Current file: [{0}]; app = [{1}]", path, this.app.DriverAppID);

            if (!File.Exists(path))
            {
                logger.Debug("Current file does not exist.");
                md5 = null;
                return false;
            }

            md5 = File.ReadAllText(path);
            logger.Debug("Current MD5: [{0}]", md5);
            return true;
        }

        private string PrepareInstallationPackage(out string md5)
        {
            string packageDir = this.PackageDir;
            var caps = this.app.Capabilities;

            // Quick comparison
            string filename;
            if (caps.MD5 != null && this.TryReadPackageInfo(out filename, out md5))
            {
                if (md5 == caps.MD5.ToLower())
                {
                    this.prepared = true;
                    return Path.Combine(packageDir, filename);
                }
            }

            // Download the installer
            var installer = this.utils.GetAppFileFromWeb(caps.App, caps.MD5);
            filename = Path.GetFileName(installer); // TODO Preserve original filename
            md5 = caps.MD5 != null ? caps.MD5.ToLower() : this.utils.GetFileMD5(installer);

            // Discard existing package
            if (Directory.Exists(packageDir))
            {
                Directory.Delete(packageDir, true);
            }
            Directory.CreateDirectory(packageDir);

            File.Move(installer, Path.Combine(packageDir, filename));
            this.WritePackageInfo(filename, md5);
            string[] lines = { filename, md5 };

            this.prepared = true;
            return null;
        }

        private bool TryReadPackageInfo(out string filename, out string md5)
        {
            string path = this.PackageInfoFile;
            logger.Debug("Package info file: [{0}]; app = [{1}]", path, this.app.DriverAppID);
            if (!File.Exists(path))
            {
                logger.Debug("Package info file does not exist.");
                filename = null;
                md5 = null;
                return false;
            }

            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            filename = lines[0];
            md5 = lines[1];

            logger.Debug("Package info: filename = [{0}], MD5 = [{1}]", filename, md5);
            return true;
        }

        private void WritePackageInfo(string filename, string md5)
        {
            string path = this.PackageInfoFile;
            string[] lines = { filename, md5 };
            logger.Debug(
                "Write package info; app = [{0}], path = [{1}], filename = [{2}], MD5 = [{3}]",
                this.app.DriverAppID, path, filename, md5);

            File.WriteAllLines(path, lines, Encoding.UTF8);
        }
    }
}