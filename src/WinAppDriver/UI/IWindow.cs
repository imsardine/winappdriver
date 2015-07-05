namespace WinAppDriver.UI
{
    using System;
    using System.Windows;

    internal interface IWindow
    {
        IntPtr Handle { get; }

        Point Location { get; }

        Size Size { get; }

        void BringToFront();

        void Close();
    }
}