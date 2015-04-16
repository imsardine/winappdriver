namespace WinAppDriver.Desktop
{
    using System.Collections.Generic;
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

        public bool IsBuildChanged()
        {
            string currentChecksum;
            if (!this.TryReadCurrent(out currentChecksum))
            {
                logger.Info(
                    "Build changed, because current build (checksum) is unknown; app = [{0}]",
                    this.app.DriverAppID);
                return true;
            }

            string cachedChecksum;
            string package = this.PrepareInstallationPackage(out cachedChecksum);
            bool changed = currentChecksum != cachedChecksum;

            logger.Info(
                "Build changed? {0}; app = [{1}], current checksum = [{2}], cached checksum = [{3}]",
                changed, this.app.DriverAppID, currentChecksum, cachedChecksum);
            return changed;
        }

        public void Install()
        {
            string checksum;
            string package = this.PrepareInstallationPackage(out checksum);

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
                this.utils.Execute(command, envs, out exitCode);
                this.UpdateCurrent(checksum);
            }
            finally
            {
                this.uacHandler.Deactivate();
            }
        }

        private bool TryReadCurrent(out string checksum)
        {
            string path = this.CurrentFile;
            logger.Debug("Current file: [{0}]; app = [{1}]", path, this.app.DriverAppID);

            if (!File.Exists(path))
            {
                logger.Debug("Current file does not exist.");
                checksum = null;
                return false;
            }

            checksum = File.ReadAllText(path);
            logger.Debug("Current build (checksum): [{0}]", checksum);
            return true;
        }

        private void UpdateCurrent(string checksum)
        {
            string path = this.CurrentFile;
            checksum = checksum.ToLower();
            logger.Debug("Update current build (checksum); app = [{0}], checksum = [{1}]", this.app.DriverAppID, checksum);

            File.WriteAllText(path, checksum);
        }

        private string PrepareInstallationPackage(out string checksum)
        {
            string baseDir = this.PackageDir;
            var caps = this.app.Capabilities;
            logger.Debug(
                "Prepare installation package; app = [{0}], base dir = [{1}], caps.App = [{2}], caps.AppChecksum = [{3}]",
                this.app.DriverAppID, baseDir, caps.App, caps.AppChecksum);

            string filename;
            string fullpath;
            bool cached = this.TryReadCachedPackageInfo(out filename, out checksum);
            if (this.prepared && cached)
            {
                fullpath = Path.Combine(baseDir, filename);
                logger.Debug(
                    "Alread prepared; path = [{0}], checksum = [{1}]", fullpath, checksum);
                return fullpath;
            }

            // Quick comparison
            if (caps.AppChecksum != null && cached)
            {
                logger.Debug("Checksum matching (case-insensitive); caps.AppChecksum = [{0}], cached checksum = [{1}]", caps.AppChecksum, checksum);
                fullpath = Path.Combine(baseDir, filename);
                if (checksum == caps.AppChecksum.ToLower())
                {
                    logger.Info(
                        "The cached installation package of app '{0}' ({1}) can be reused, because of matched checksums.",
                        this.app.DriverAppID, fullpath);

                    this.prepared = true;
                    return fullpath;
                }
                else
                {
                    logger.Info(
                        "The cached installation package of app '{0}' ({1}) can not be reused, because of unmatched checksums.",
                        this.app.DriverAppID, fullpath);
                }
            }

            // Download the installation package
            logger.Info(
                "Start downloading installation pacakge of app '{0}' from {1}.",
                this.app.DriverAppID, caps.App);
            string downloaded = this.utils.GetAppFileFromWeb(caps.App, caps.AppChecksum);
            filename = Path.GetFileName(downloaded); // TODO Preserve original filename, and replace invalid characters
            checksum = caps.AppChecksum != null ? caps.AppChecksum.ToLower() : this.utils.GetFileMD5(downloaded);
            logger.Info("Installation package downloaded: {0} ({1}).", downloaded, checksum);

            // Discard cached installation package
            if (Directory.Exists(baseDir))
            {
                Directory.Delete(baseDir, true);
            }

            Directory.CreateDirectory(baseDir);

            // Update cached package information.
            fullpath = Path.Combine(baseDir, filename);
            logger.Info("Cache the installation package: {0}.", fullpath);
            File.Move(downloaded, fullpath);
            this.WriteCachedPackageInfo(filename, checksum);

            this.prepared = true;
            return fullpath;
        }

        private bool TryReadCachedPackageInfo(out string filename, out string checksum)
        {
            string path = this.PackageInfoFile;
            logger.Debug("Cached package info file: [{0}]; app = [{1}]", path, this.app.DriverAppID);
            if (!File.Exists(path))
            {
                logger.Debug("Cached package info file does not exist.");
                filename = null;
                checksum = null;
                return false;
            }

            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            filename = lines[0];
            checksum = lines[1];

            logger.Debug("Cached package info: filename = [{0}], checksum = [{1}]", filename, checksum);
            return true;
        }

        private void WriteCachedPackageInfo(string filename, string checksum)
        {
            string path = this.PackageInfoFile;
            string[] lines = { filename, checksum };
            logger.Debug(
                "Write cached package info; app = [{0}], path = [{1}], filename = [{2}], checksum = [{3}]",
                this.app.DriverAppID, path, filename, checksum);

            File.WriteAllLines(path, lines, Encoding.UTF8);
        }
    }
}