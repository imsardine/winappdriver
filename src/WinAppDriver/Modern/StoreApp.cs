namespace WinAppDriver.Modern
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Management.Automation;
    using System.Runtime.InteropServices;
    using System.Xml;

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed.")]
    internal class StoreApp : IStoreApp
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IDriverContext context;

        private Capabilities capabilities;

        private AppInfo infoCache;

        private IPackageInstaller installerCache;

        private IUtils utils;

        public StoreApp(IDriverContext context, Capabilities capabilities, IUtils utils)
        {
            // TODO verify capabilities
            this.context = context;
            this.capabilities = capabilities;
            this.utils = utils;
        }

        public string DriverAppID
        {
            get { return this.PackageName + " (Modern)"; }
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
                    this.installerCache = new StoreAppInstaller(this.context, this, this.utils);
                }

                return this.installerCache;
            }
        }

        public string PackageName
        {
            get
            {
                return this.capabilities.PackageName;
            }
        }

        public string AppUserModelId
        {
            get { return this.GetInstalledAppInfo().AppUserModelId; }
        }

        public string PackageFamilyName
        {
            get { return this.GetInstalledAppInfo().PackageFamilyName; }
        }

        public string PackageFullName
        {
            get { return this.GetInstalledAppInfo().PackageFullName; }
        }

        public string PackageFolderDir
        {
            get
            {
                return this.utils.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Packages\" + this.PackageFamilyName);
            }
        }

        public void Uninstall()
        {
            PowerShell ps = PowerShell.Create();
            ps.AddCommand("Remove-AppxPackage");
            ps.AddArgument(this.PackageFullName);
            ps.Invoke();
            this.utils.DeleteDirectoryIfExists(this.StatesDir);
        }

        public bool IsInstalled()
        {
            if (this.infoCache != null)
            {
                return true;
            }
            else
            {
                return this.TryGetInstalledAppInfo(out this.infoCache);
            }
        }

        public void Activate()
        {
            logger.Info(
                "Activate the store app; current working directory = [{0}], " +
                "AppUserModelID = [{1}].",
                Environment.CurrentDirectory, this.AppUserModelId);

            var info = new ProcessStartInfo(
                Path.Combine(Environment.CurrentDirectory, "ActivateStoreApp.exe"),
                this.AppUserModelId);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            var process = Process.Start(info);
            logger.Debug("PID of ActivateStoreApp.exe = {0}.", process.Id);
            process.WaitForExit(5 * 1000);

            if (process.ExitCode == 0)
            {
                logger.Debug("STDOUT = [{0}].", process.StandardOutput.ReadToEnd());
            }
            else
            {
                string msg = string.Format(
                    "Error occurred while activating the store app; " +
                    "code = {0}, STDOUT = [{1}], STDERR = [{2}].",
                    process.ExitCode,
                    process.StandardOutput.ReadToEnd(),
                    process.StandardError.ReadToEnd());
                throw new WinAppDriverException(msg);
            }
        }

        public void Terminate()
        {
            var api = (IPackageDebugSettings)new PackageDebugSettings();
            api.TerminateAllProcesses(this.PackageFullName);
        }

        public bool BackupInitialStates(bool overwrite)
        {
            if (this.utils.DirectoryExists(this.StatesDir) && !overwrite)
            {
                return false;
            }

            if (overwrite)
            {
                this.utils.DeleteDirectoryIfExists(this.StatesDir);
            }

            this.utils.CopyDirectoryAndSecurity(this.PackageFolderDir, this.StatesDir);
            return true;
        }

        public void RestoreInitialStates()
        {
            var src = Path.Combine(this.StatesDir, Path.GetFileName(this.PackageFolderDir));
            this.utils.CopyDirectoryAndSecurity(src, Path.GetDirectoryName(this.PackageFolderDir));
        }

        private AppInfo GetInstalledAppInfo()
        {
            if (this.infoCache != null)
            {
                return this.infoCache;
            }

            if (this.TryGetInstalledAppInfo(out this.infoCache))
            {
                return this.infoCache;
            }
            else
            {
                throw new InvalidOperationException(string.Format(
                    "The package '{0}' is not installed yet.",
                    this.PackageName));
            }
        }

        private bool TryGetInstalledAppInfo(out AppInfo info)
        {
            string packageName, version, packageFamilyName, packageFullName, appUserModelId;

            using (var ps = PowerShell.Create())
            {
                var results = ps.AddCommand("Get-AppxPackage").AddParameter("Name", this.PackageName).Invoke();
                if (results.Count == 0)
                {
                    info = null;
                    return false;
                }

                var properties = results[0].Properties;
                packageName = (string)properties["Name"].Value; // normal
                version = (string)properties["Version"].Value;
                packageFamilyName = (string)properties["PackageFamilyName"].Value;
                packageFullName = (string)properties["PackageFullName"].Value;
            }

            using (var ps = PowerShell.Create())
            {
                var manifest = ps.AddCommand("Get-AppxPackageManifest").AddParameter("Package", packageFullName).Invoke()[0];
                var root = (XmlElement)manifest.Properties["Package"].Value;
                string appID = root["Applications"]["Application"].GetAttribute("Id");
                appUserModelId = packageFamilyName + "!" + appID;
            }

            info = new AppInfo
            {
                PackageName = packageName,
                Version = version,
                PackageFamilyName = packageFamilyName,
                PackageFullName = packageFullName,
                AppUserModelId = appUserModelId
            };

            return true;
        }

        private class AppInfo
        {
            public string PackageName { get; set; }

            public string Version { get; set; }

            public string PackageFamilyName { get; set; }

            public string PackageFullName { get; set; }

            public string AppUserModelId { get; set; }
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