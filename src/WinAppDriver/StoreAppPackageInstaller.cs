namespace WinAppDriver
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Management.Automation;

    internal class StoreAppPackageInstaller : IPackageInstaller
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IStoreApp app;
        
        private IUtils utils;

        private string source;

        private string md5Checksum;

        private string sourcePath;

        public StoreAppPackageInstaller(IStoreApp app, IUtils utils, string source, string md5Checksum)
        {
            this.app = app;
            this.utils = utils;
            this.source = source;
            this.md5Checksum = md5Checksum;
        }

        public bool IsBuildChanged()
        {
            if (this.md5Checksum != null)
            {
                string localMd5 = this.GetLocalMD5();
                if (localMd5 != this.md5Checksum)
                {
                    if (!this.source.EndsWith(".zip"))
                    {
                        throw new FailedCommandException("Your app file is \"" + this.source + "\". App file is not a .zip file.", 13);
                    }

                    this.sourcePath = this.utils.GetAppFileFromWeb(this.source, this.md5Checksum);
                    return true;
                }
                else
                {
                    logger.Info("The current installed version and the assigned version are the same, so skip installing.");
                    return false;
                }
            }
            else
            {
                if (!this.source.EndsWith(".zip"))
                {
                    throw new FailedCommandException("Your app file is \"" + this.source + "\". App file is not a .zip file.", 13);
                }

                this.sourcePath = this.utils.GetAppFileFromWeb(this.source, this.md5Checksum);
                if (this.GetLocalMD5() != this.utils.GetFileMD5(this.sourcePath))
                {
                    return true;
                }
                else
                {
                    logger.Info("The current installed version and the download version are the same, so skip installing.");
                    return false;
                }
            }
        }

        public void Install()
        {
            if (this.sourcePath == null)
            {
                if (!this.source.EndsWith(".zip"))
                {
                    throw new FailedCommandException("Your app file is \"" + this.source + "\". App file is not a .zip file.", 13);
                }

                this.sourcePath = this.utils.GetAppFileFromWeb(this.source, this.md5Checksum);
            }

            string sourceFolder = this.sourcePath.Remove(this.sourcePath.Length - 4);
            ZipFile.ExtractToDirectory(this.sourcePath, sourceFolder);
            logger.Debug("Zip file extract to: \"{0}\"", sourceFolder);

            DirectoryInfo dir = new DirectoryInfo(sourceFolder);
            FileInfo[] files = dir.GetFiles("*.ps1", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                logger.Info("Installing Windows Store App.");
                string dirs = files[0].DirectoryName;
                PowerShell ps = PowerShell.Create();
                ps.AddScript(@"Powershell.exe -executionpolicy remotesigned -NonInteractive -File " + files[0].FullName);
                ps.Invoke();
                System.Threading.Thread.Sleep(1000); // Waiting activity done.

                this.StoreMD5(this.utils.GetFileMD5(this.sourcePath));
            }
            else
            {
                throw new FailedCommandException("Cannot find .ps1 file in \"" + sourceFolder + "\".", 13);
            }
        }

        private string GetLocalMD5()
        {
            string md5FileName = System.IO.Path.Combine(this.app.PackageFolderDir, "MD5.txt");
            if (System.IO.File.Exists(md5FileName))
            {
                System.IO.StreamReader fileReader = System.IO.File.OpenText(md5FileName);
                logger.Debug("Getting MD5 from file: \"{0}\".", md5FileName);

                return fileReader.ReadLine().ToString();
            }
            else
            {
                return null;
            }
        }

        private void StoreMD5(string fileMD5)
        {
            string md5FileName = System.IO.Path.Combine(this.app.PackageFolderDir, "MD5.txt");
            using (System.IO.FileStream fs = System.IO.File.Create(md5FileName))
            {
                logger.Debug("Writing MD5 to file: \"{0}\"", md5FileName);
                byte[] byteMD5 = System.Text.Encoding.Default.GetBytes(fileMD5);
                for (int i = 0; i < byteMD5.Length; i++)
                {
                    fs.WriteByte(byteMD5[i]);
                }
            }
        }
    }
}
