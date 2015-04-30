namespace WinAppDriver.Modern
{
    using System.IO;
    using System.IO.Compression;
    using System.Management.Automation;

    internal class StoreAppInstaller : AppInstaller<IStoreApp>
    {
        public StoreAppInstaller(IDriverContext context, IStoreApp app, IUtils utils)
            : base(context, app, utils)
        {
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
                string dirs = files[0].DirectoryName;
                PowerShell ps = PowerShell.Create();
                ps.AddScript(string.Format("Powershell.exe -executionpolicy remotesigned -NonInteractive -File \"{0}\"", files[0].FullName));
                ps.Invoke(); // TODO error handling
            }
            else
            {
                throw new FailedCommandException("Cannot find .ps1 file in \"" + sourceFolder + "\".", 13);
            }
        }
    }
}