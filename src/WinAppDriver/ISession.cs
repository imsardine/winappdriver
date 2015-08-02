namespace WinAppDriver
{
    using System.Windows.Automation;

    internal interface ISession
    {
        string ID { get; }

        IApplication Application { get; }

        Capabilities Capabilities { get; }

        bool FocusOnCurrentWindow { get; set; }

        int ImplicitWaitMillis { get; set; }

        int AddUIElement(AutomationElement element);

        AutomationElement GetUIElement(int id);
    }
}