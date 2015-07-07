namespace WinAppDriver.UI
{
    using System.Windows.Automation;

    internal class ElementFactory : IElementFactory
    {
        public IElement GetElement(AutomationElement element)
        {
            return new Element(element);
        }
    }
}