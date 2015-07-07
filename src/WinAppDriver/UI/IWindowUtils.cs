namespace WinAppDriver.UI
{
    using System;
    using System.Collections.Generic;

    internal interface IWindowUtils
    {
        IWindow GetCurrentWindow();

        ISet<IntPtr> GetTopLevelWindowHandles();

        ISet<IWindow> GetTopLevelWindows();
    }
}