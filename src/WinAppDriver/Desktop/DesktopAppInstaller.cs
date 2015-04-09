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
            string currentMD5;
            if (!this.TryReadCurrent(out currentMD5))
            {
                logger.Info(
                    "Build changed, because current build (MD5) is unknown; app = [{0}]",
                    this.app.DriverAppID);
                return true;
            }

            string cachedMD5;
            string package = this.PrepareInstallationPackage(out cachedMD5);
            bool changed = currentMD5 != cachedMD5;

            logger.Info(
                "Build changed? {0}; app = [{1}], current MD5 = [{2}], cached MD5 = [{3}]",
                changed, this.app.DriverAppID, currentMD5, cachedMD5);
            return changed;
        }

        public void Install()
        {
            string md5;
            string package = this.PrepareInstallationPackage(out md5);

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
                    "Install command is not provided, so directly invoke the (executable?) installation package '{1}'.",
                    package);
            }

            var envs = new Dictionary<string, string>
            {
                { "AppInstallationPackage", package }
            };

            try
            {
                this.uacHandler.Activate(true);
                this.utils.Execute(command, envs, true);
            }
            finally
            {
                this.uacHandler.Deactivate();
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
            logger.Debug("Current build (MD5): [{0}]", md5);
            return true;
        }

        private void UpdateCurrent(string md5)
        {
            string path = this.CurrentFile;
            md5 = md5.ToLower();
            logger.Debug("Update current build (MD5); app = [{0}], md5 = [{1}]", this.app.DriverAppID, md5);

            File.WriteAllText(path, md5);
        }

        private string PrepareInstallationPackage(out string md5)
        {
            string baseDir = this.PackageDir;
            var caps = this.app.Capabilities;
            logger.Debug(
                "Prepare installation package; app = [{0}], base dir = [{1}], caps.App = [{2}], caps.MD5 = [{3}]",
                this.app.DriverAppID, baseDir, caps.App, caps.MD5);

            string filename;
            string fullpath;
            bool cached = this.TryReadCachedPackageInfo(out filename, out md5);
            if (this.prepared && cached)
            {
                fullpath = Path.Combine(baseDir, filename);
                logger.Debug(
                    "Alread prepared; path = [{1}], MD5 = [{2}]", fullpath, md5);
                return fullpath;
            }

            // Quick comparison
            if (caps.MD5 != null && cached)
            {
                logger.Debug("MD5 matching (case-insensitive); caps.MD5 = [{0}], cached MD5 = [{1}]", caps.MD5, md5);
                fullpath = Path.Combine(baseDir, filename);
                if (md5 == caps.MD5.ToLower())
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
            string downloaded = this.utils.GetAppFileFromWeb(caps.App, caps.MD5);
            filename = Path.GetFileName(downloaded); // TODO Preserve original filename, and replace invalid characters
            md5 = caps.MD5 != null ? caps.MD5.ToLower() : this.utils.GetFileMD5(downloaded);
            logger.Info("Installation package downloaded: {0} ({1}).", downloaded, md5);

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
            this.WriteCachedPackageInfo(filename, md5);

            this.prepared = true;
            return fullpath;
        }

        private bool TryReadCachedPackageInfo(out string filename, out string md5)
        {
            string path = this.PackageInfoFile;
            logger.Debug("Cached package info file: [{0}]; app = [{1}]", path, this.app.DriverAppID);
            if (!File.Exists(path))
            {
                logger.Debug("Cached package info file does not exist.");
                filename = null;
                md5 = null;
                return false;
            }

            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            filename = lines[0];
            md5 = lines[1];

            logger.Debug("Cached package info: filename = [{0}], MD5 = [{1}]", filename, md5);
            return true;
        }

        private void WriteCachedPackageInfo(string filename, string md5)
        {
            string path = this.PackageInfoFile;
            string[] lines = { filename, md5 };
            logger.Debug(
                "Write cached package info; app = [{0}], path = [{1}], filename = [{2}], MD5 = [{3}]",
                this.app.DriverAppID, path, filename, md5);

            File.WriteAllLines(path, lines, Encoding.UTF8);
        }
    }
}