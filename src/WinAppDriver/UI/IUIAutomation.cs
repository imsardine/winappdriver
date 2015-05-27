namespace WinAppDriver.UI
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Automation;

    internal interface IUIAutomation
    {
        AutomationElement GetFocusedWindowOrRoot();

        ISet<string> GetTopLevelWindowHandles();

        ISet<AutomationElement> GetTopLevelWindows();

        IntPtr ToNativeWindowHandle(AutomationElement element);

        AutomationElement FromNativeWindowHandle(IntPtr handle);

        string DumpXml(AutomationElement start);

        string DumpXml(AutomationElement start, out IList<AutomationElement> elements);

        AutomationElement FindFirstByXPath(AutomationElement start, string xpath);

        IList<AutomationElement> FindAllByXPath(AutomationElement start, string xpath);
    }
}