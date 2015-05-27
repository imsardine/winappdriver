namespace WinAppDriver.UI
{
    using System;

    internal interface IWindowFactory
    {
        IWindow GetWindow(IntPtr handle);
    }
}