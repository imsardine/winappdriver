namespace WinAppDriver.Modern
{
    using System.IO;
    using System.IO.Compression;
    using System.Management.Automation;

    internal class StoreAppInstaller : AppInstaller<IStoreApp>
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IStoreApp app;

        private IUtils utils;

        public StoreAppInstaller(IDriverContext context, IStoreApp app, IUtils utils)
            : base(context, app, utils)
        {
            this.app = app;
            this.utils = utils;
        }

        protected internal override void InstallImpl(string package, string checksum)
        {
            string sourceFolder = package.Remove(package.Length - 4);
            ZipFile.ExtractToDirectory(package, sourceFolder); // TODO use temp dir instead
            logger.Debug("Zip file extract to: \"{0}\"", sourceFolder);

            DirectoryInfo dir = new DirectoryInfo(sourceFolder);
            FileInfo[] files = dir.GetFiles("*.ps1", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                logger.Info("Installing Windows Store App.");
                var command = string.Format(
                    "Powershell.exe -executionpolicy remotesigned -NonInteractive -File \"{0}\"", 
                    files[0].FullName);
                logger.Debug("Command: {0}", command);

                var process = this.utils.Execute(command, null);
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new FailedCommandException(
                        string.Format("Failed to install the store app. exit = {0}.", process.ExitCode), 13);
                }
            }
            else
            {
                throw new FailedCommandException("Cannot find .ps1 file in \"" + sourceFolder + "\".", 13);
            }
        }
    }
}