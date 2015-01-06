namespace WinAppDriver
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    internal interface IStoreApplication : IApplication
    {
        string AppUserModelId { get; }

        string PackageFamilyName { get; }

        string GetPackageFullName();
    }

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed.")]
    internal class StoreApplication : IStoreApplication
    {
        private string packageFamilyNameCache = null;

        public StoreApplication(string appUserModelId)
        {
            this.AppUserModelId = appUserModelId;
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
                            "Invalid Application User Model ID: {0)",
                            this.AppUserModelId);
                        throw new WinAppDriverException(msg);
                    }

                    this.packageFamilyNameCache = this.AppUserModelId.Substring(0, index - 1);
                }

                return this.packageFamilyNameCache;
            }
        }

        public bool IsInstalled()
        {
            return true; // TODO
        }

        public string GetPackageFullName()
        {
            return "2DE213C9.KKBOX_1.0.0.16_x64__wttxem0f9q9s0"; // TODO Get name from PS output
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

        public void Reset()
        {
            throw new NotImplementedException();
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