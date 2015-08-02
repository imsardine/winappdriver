namespace WinAppDriver.UI
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Automation;

    internal interface IUIAutomation
    {
        IElement FocusedElement { get; }

        AutomationElement GetFocusedWindowOrRoot();

        ISet<AutomationElement> GetTopLevelWindows();

        IntPtr ToNativeWindowHandle(AutomationElement element);

        AutomationElement FromNativeWindowHandle(IntPtr handle);

        string DumpXml(AutomationElement start);

        string DumpXml(AutomationElement start, out IList<AutomationElement> elements);

        ControlType FromTagName(string tag);

        string ToTagName(ControlType type);

        AutomationElement FindFirstByXPath(AutomationElement start, string xpath);

        IList<AutomationElement> FindAllByXPath(AutomationElement start, string xpath);

        IElement GetScrollableContainer(IElement element);
    }
}