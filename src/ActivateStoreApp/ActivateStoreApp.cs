using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: ActivateStoreApp <AppUserModelId>");
            return 1;
        }

        ApplicationActivationManager appActiveManager = new ApplicationActivationManager();
        uint pid;
        appActiveManager.ActivateApplication(args[0], null, ActivateOptions.None, out pid);
        Console.WriteLine("Process ID: {0}", pid);
        return 0;
    }
}

public enum ActivateOptions
{
    None = 0x00000000,
    DesignMode = 0x00000001,
    NoErrorUI = 0x00000002,
    NoSplashScreen = 0x00000004,
}

[ComImport, Guid("2e941141-7f97-4756-ba1d-9decde894a3d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IApplicationActivationManager
{
    IntPtr ActivateApplication([In] String appUserModelId,
                               [In] String arguments,
                               [In] ActivateOptions options,
                               [Out] out UInt32 processId);

    IntPtr ActivateForFile([In] String appUserModelId,
                  [In] IntPtr /*IShellItemArray* */ itemArray,
                  [In] String verb,
                  [Out] out UInt32 processId);

    IntPtr ActivateForProtocol([In] String appUserModelId,
                  [In] IntPtr /* IShellItemArray* */itemArray,
                  [Out] out UInt32 processId);
}

[ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]
//Application Activation Manager
internal class ApplicationActivationManager : IApplicationActivationManager
{
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType =
                        MethodCodeType.Runtime)/*, PreserveSig*/]
    public extern IntPtr ActivateApplication(
                               [In] String appUserModelId,
                               [In] String arguments,
                               [In] ActivateOptions options,
                               [Out] out UInt32 processId);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType =
                                         MethodCodeType.Runtime)]
    public extern IntPtr ActivateForFile(
                          [In] String appUserModelId,
                          [In] IntPtr /*IShellItemArray* */itemArray,
                          [In] String verb,
                          [Out] out UInt32 processId);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType =
                                          MethodCodeType.Runtime)]
    public extern IntPtr ActivateForProtocol(
                         [In] String appUserModelId,
                         [In] IntPtr /* IShellItemArray* */itemArray,
                         [Out] out UInt32 processId);
}