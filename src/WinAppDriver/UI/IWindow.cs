namespace WinAppDriver.UI
{
    using System;
    using System.Windows;

    internal interface IWindow // TODO extends IElement
    {
        IntPtr Handle { get; }

        Point Location { get; }

        Size Size { get; }

        void BringToFront();

        void Close();
    }
}