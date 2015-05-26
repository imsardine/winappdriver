namespace WinAppDriver
{
    using System.Collections.Generic;
    using System.Windows.Automation;

    internal interface IUIAutomation
    {
        AutomationElement GetFocusedWindowOrRoot();

        ISet<AutomationElement> GetTopLevelWindows();

        string DumpXml(AutomationElement start);

        string DumpXml(AutomationElement start, out IList<AutomationElement> elements);

        AutomationElement FindFirstByXPath(AutomationElement start, string xpath);

        IList<AutomationElement> FindAllByXPath(AutomationElement start, string xpath);
    }
}