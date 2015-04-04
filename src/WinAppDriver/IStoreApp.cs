namespace WinAppDriver
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Management.Automation;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal interface IStoreApp : IApplication
    {
        string PackageName { get; }

        string AppUserModelId { get; }

        string PackageFamilyName { get; }

        string PackageFullName { get; }

        string PackageFolderDir { get; }
    }

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed.")]
    internal class StoreApp : IStoreApp
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private Capabilities capabilities;

        private AppInfo infoCache;

        private IPackageInstaller installerCache;

        private IUtils utils;

        public StoreApp(Capabilities capabilities, IUtils utils)
        {
            // TODO verify capabilities
            this.capabilities = capabilities;
            this.utils = utils;
        }

        public Capabilities Capabilities
        {
            get { return this.capabilities; }
        }

        public IPackageInstaller Installer
        {
            get
            {
                if (this.installerCache == null)
                {
                    this.installerCache = new StoreAppPackageInstaller(this, this.utils, this.capabilities.App, this.capabilities.MD5);
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
            this.utils.DeleteDirectory(this.InitialStatesDir);
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

        public void Launch()
        {
            if (Directory.Exists(this.InitialStatesDir))
            {
                this.RestoreInitialStates();
            }
            else
            {
                this.BackupInitialStates();
            }

            this.Activate();
        }

        public void Activate()
        {
            // TODO thorw exception if needed
            Process.Start("ActivateStoreApp", this.AppUserModelId);
        }

        public void Terminate()
        {
            var api = (IPackageDebugSettings)new PackageDebugSettings();
            api.TerminateAllProcesses(this.PackageFullName);
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

        private string InitialStatesDir
        {
            get
            {
                return this.utils.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\WinAppDriver\Packages\" + this.PackageFamilyName);
            }
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