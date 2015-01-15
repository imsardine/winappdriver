namespace WinAppDriver
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Management.Automation;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    internal interface IStoreApplication : IApplication
    {
        string AppUserModelId { get; }

        string PackageFamilyName { get; }

        string PackageFolderDir { get; }

        string GetPackageFullName();

        string GetLocalMD5();

        string GetFileMD5(string filePath);

        string GetAppFileFromWeb(string webResource, string expectFileMD5);

        void UninstallApp(string packageFullName);

        void InstallApp(string zipFile);
    }

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed.")]
    internal class StoreApplication : IStoreApplication
    {
        private string packageNameCache = null;

        private string packageFamilyNameCache = null;

        private string packageFolderDirCache = null;

        private string initialStatesDirCache = null;

        private IUtils utils;

        public StoreApplication(string appUserModelId, IUtils utils)
        {
            this.AppUserModelId = appUserModelId;
            this.utils = utils;
        }

        public string AppUserModelId
        {
            get;
            private set;
        }

        public string PackageFamilyName
        {
            get
            {
                if (this.packageFamilyNameCache == null)
                {
                    // AppUserModelId = {PackageFamilyName}!{AppId}
                    int index = this.AppUserModelId.IndexOf('!');
                    if (index == -1)
                    {
                        string msg = string.Format(
                            "Invalid Application User Model ID: {0}",
                            this.AppUserModelId);
                        throw new WinAppDriverException(msg);
                    }

                    this.packageFamilyNameCache = this.AppUserModelId.Substring(0, index);
                }

                return this.packageFamilyNameCache;
            }
        }

        public string PackageFolderDir
        {
            get
            {
                if (this.packageFolderDirCache == null)
                {
                    this.packageFolderDirCache = this.utils.ExpandEnvironmentVariables(
                        @"%LOCALAPPDATA%\Packages\" + this.PackageFamilyName);
                }

                return this.packageFolderDirCache;
            }
        }

        public string GetPackageFullName()
        {
            if (this.IsInstalled())
            {
                // PackageFamilyName = {Name}_{PublisherHash}!{AppId}
                PowerShell ps = PowerShell.Create();
                ps.AddCommand("Get-AppxPackage");
                ps.AddParameter("Name", this.PackageName);
                System.Collections.ObjectModel.Collection<PSObject> package = ps.Invoke();

                return package[0].Members["PackageFullName"].Value.ToString();
            }
            else
            {
                string msg = string.Format("Application is not installed, cannot find PackageFullName.");
                throw new InvalidOperationException(msg);
            }
        }

        public string GetLocalMD5()
        {
            string md5FileName = System.IO.Path.Combine(this.PackageFolderDir, "MD5.txt");
            if (System.IO.File.Exists(md5FileName))
            {
                System.IO.StreamReader fileReader = System.IO.File.OpenText(md5FileName);
                Console.Out.WriteLine("Getting MD5 from file: \"{0}\".", md5FileName);

                return fileReader.ReadLine().ToString();
            }
            else
            {
                return null;
            }
        }

        public string GetFileMD5(string filePath)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            FileStream zipFile = new FileStream(filePath, FileMode.Open);
            byte[] bytes = md5.ComputeHash(zipFile);
            zipFile.Close();
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
        }

        public string GetAppFileFromWeb(string webResource, string expectFileMD5)
        {
            string storeFileName = Environment.GetEnvironmentVariable("TEMP") + @"\StoreApp_" + DateTime.Now.ToString("yyyyMMddHHmmss") + webResource.Substring(webResource.LastIndexOf("."));

            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();

            Console.WriteLine("Downloading File \"{0}\" .......\n\n", webResource);

            // Download the Web resource and save it into temp folder.
            myWebClient.DownloadFile(webResource, storeFileName);
            Console.WriteLine("Successfully Downloaded File \"{0}\"", webResource);
            Console.WriteLine("\nDownloaded file saved in the following file system folder:\n\t" + storeFileName);

            string fileMD5 = this.GetFileMD5(storeFileName);
            if (expectFileMD5 != null && expectFileMD5 != this.GetFileMD5(storeFileName))
            {
                string msg = "You got a wrong file. ExpectMD5 is \"" + expectFileMD5 + "\", but download file MD5 is \"" + fileMD5 + "\".";
                throw new WinAppDriverException(msg);
            }

            return storeFileName;
        }

        public void UninstallApp(string packageFullName)
        {
            PowerShell ps = PowerShell.Create();
            ps.AddCommand("Remove-AppxPackage");
            ps.AddArgument(packageFullName);
            ps.Invoke();
        }

        public void InstallApp(string zipFile)
        {
            string fileFolder = zipFile.Remove(zipFile.Length - 4);
            ZipFile.ExtractToDirectory(zipFile, fileFolder);
            Console.WriteLine("\nZip file extract to:\n\t" + fileFolder);

            DirectoryInfo dir = new DirectoryInfo(fileFolder);
            FileInfo[] files = dir.GetFiles("*.ps1", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                Console.WriteLine("\nInstalling Windows Store App. \n");
                string dirs = files[0].DirectoryName;
                PowerShell ps = PowerShell.Create();
                ps.AddScript(@"Powershell.exe -executionpolicy remotesigned -NonInteractive -File " + files[0].FullName);
                ps.Invoke();
                System.Threading.Thread.Sleep(1000); // Waiting activity done.

                this.StoreMD5(this.GetFileMD5(zipFile));
                this.BackupInitialStates();
            }
            else
            {
                throw new FailedCommandException("Cannot find .ps1 file in \"" + fileFolder + "\".", 13);
            }
        }

        public bool IsInstalled()
        {
            PowerShell ps = PowerShell.Create();
            ps.AddCommand("Get-AppxPackage");
            ps.AddParameter("Name", this.PackageName);
            System.Collections.ObjectModel.Collection<PSObject> package = ps.Invoke();
            if (package.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Activate()
        {
            // TODO thorw exception if needed
            Process.Start("ActivateStoreApp", this.AppUserModelId);
        }

        public void Terminate()
        {
            var api = (IPackageDebugSettings)new PackageDebugSettings();
            api.TerminateAllProcesses(this.GetPackageFullName());
        }

        public void BackupInitialStates()
        {
            this.utils.CopyDirectory(this.PackageFolderDir + @"\Settings", this.InitialStatesDir + @"\Settings");
            this.utils.CopyDirectory(this.PackageFolderDir + @"\LocalState", this.InitialStatesDir + @"\LocalState");
        }

        public void RestoreInitialStates()
        {
            this.utils.CopyDirectory(this.InitialStatesDir + @"\Settings", this.PackageFolderDir + @"\Settings");
            this.utils.CopyDirectory(this.InitialStatesDir + @"\LocalState", this.PackageFolderDir + @"\LocalState");
        }

        private string PackageName
        {
            get
            {
                if (this.packageNameCache == null)
                {
                    // PackageFamilyName = {Name}_{PublisherHash}!{AppId}
                    this.packageNameCache = this.PackageFamilyName.Remove(this.PackageFamilyName.IndexOf("_"));
                }

                return this.packageNameCache;
            }
        }

        private string InitialStatesDir
        {
            get
            {
                if (this.initialStatesDirCache == null)
                {
                    this.initialStatesDirCache = this.utils.ExpandEnvironmentVariables(
                        @"%LOCALAPPDATA%\WinAppDriver\Packages\" + this.PackageFamilyName);
                }

                return this.initialStatesDirCache;
            }
        }

        private void StoreMD5(string fileMD5)
        {
            string md5FileName = System.IO.Path.Combine(this.PackageFolderDir, "MD5.txt");
            using (System.IO.FileStream fs = System.IO.File.Create(md5FileName))
            {
                Console.Out.WriteLine("Writing MD5 to file: \"{0}\"", md5FileName);
                byte[] byteMD5 = System.Text.Encoding.Default.GetBytes(fileMD5);
                for (int i = 0; i < byteMD5.Length; i++)
                {
                    fs.WriteByte(byteMD5[i]);
                }
            }
        }

        [ComImport, Guid("B1AEC16F-2383-4852-B0E9-8F0B1DC66B4D")]
        private class PackageDebugSettings
        {
        }

        private enum PACKAGE_EXECUTION_STATE
        {
            PES_UNKNOWN,
            PES_RUNNING,
            PES_SUSPENDING,
            PES_SUSPENDED,
            PES_TERMINATED
        }

        [ComImport, Guid("F27C3930-8029-4AD1-94E3-3DBA417810C1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed.")]
        private interface IPackageDebugSettings
        {
            int EnableDebugging(
                [MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                [MarshalAs(UnmanagedType.LPWStr)] string debuggerCommandLine,
                IntPtr environment);

            int DisableDebugging([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);

            int Suspend([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);

            int Resume([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);

            int TerminateAllProcesses([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);

            int SetTargetSessionId(int sessionId);

            int EnumerageBackgroundTasks(
                [MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                out uint taskCount,
                out int intPtr,
                [Out] string[] array);

            int ActivateBackgroundTask(IntPtr something);

            int StartServicing([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);

            int StopServicing([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);

            int StartSessionRedirection(
                [MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                uint sessionId);

            int StopSessionRedirection([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);

            int GetPackageExecutionState(
                [MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                out PACKAGE_EXECUTION_STATE packageExecutionState);

            int RegisterForPackageStateChanges(
                [MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                IntPtr pPackageExecutionStateChangeNotification,
                out uint pdwCookie);

            int UnregisterForPackageStateChanges(uint dwCookie);
        }
    }
}