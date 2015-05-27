namespace WinAppDriver.UI
{
    using System;

    internal interface IWindow
    {
        IntPtr Handle { get; }

        void BringToFront();

        void Close();
    }
}