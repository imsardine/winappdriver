namespace WinAppDriver
{
    using System.Collections.Generic;
    using System.Windows.Automation;

    internal interface IUIAutomation
    {
        string DumpXml(AutomationElement root);

        string DumpXml(AutomationElement root, out IList<AutomationElement> elements);

        AutomationElement FindFirstByXPath(AutomationElement root, string xpath);

        IList<AutomationElement> FindAllByXPath(AutomationElement root, string xpath);
    }
}