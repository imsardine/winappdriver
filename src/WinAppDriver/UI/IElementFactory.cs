namespace WinAppDriver.UI
{
    using System.Windows.Automation;

    internal interface IElementFactory
    {
        IElement GetElement(AutomationElement element);
    }
}