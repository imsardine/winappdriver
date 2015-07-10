namespace WinAppDriver.UI
{
    using System.Windows.Automation;

    internal class ElementFactory : IElementFactory
    {
        private IUIAutomation uiAutomation;

        public void SetUIAutomation(IUIAutomation uiAutomation)
        {
            this.uiAutomation = uiAutomation;
        }

        public IElement GetElement(AutomationElement element)
        {
            return new Element(element, this.uiAutomation);
        }
    }
}