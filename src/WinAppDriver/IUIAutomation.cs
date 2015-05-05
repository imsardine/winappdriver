namespace WinAppDriver
{
    using System.Collections.Generic;
    using System.Windows.Automation;

    internal interface IUIAutomation
    {
        bool TryGetFocusedWindowOrRoot(out AutomationElement window);

        string DumpXml(AutomationElement start);

        string DumpXml(AutomationElement start, out IList<AutomationElement> elements);

        AutomationElement FindFirstByXPath(AutomationElement start, string xpath);

        IList<AutomationElement> FindAllByXPath(AutomationElement start, string xpath);
    }
}