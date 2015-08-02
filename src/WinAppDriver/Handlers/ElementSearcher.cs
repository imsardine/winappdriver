namespace WinAppDriver.Handlers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Automation;
    using WinAppDriver.UI;

    internal class ElementSearcher : IElementSearcher
    {
        private IUIAutomation uiAutomation;

        private IElementFactory elementFactory;

        public ElementSearcher(IUIAutomation uiAutomation, IElementFactory elementFactory)
        {
            this.uiAutomation = uiAutomation;
            this.elementFactory = elementFactory;
        }

        public IElement FindFirst(IElement context, LocatorStrategy strategy, string locator)
        {
            AutomationProperty property = this.ToAutomationProperty(strategy);
            AutomationElement element = null;
            switch (strategy)
            {
                case LocatorStrategy.Id:
                case LocatorStrategy.Name:
                case LocatorStrategy.ClassName:
                    element = context.AutomationElement.FindFirst(
                        TreeScope.Descendants,
                        new PropertyCondition(property, locator));
                    break;

                case LocatorStrategy.TagName:
                    ControlType type = this.uiAutomation.FromTagName(locator);
                    element = context.AutomationElement.FindFirst(
                        TreeScope.Descendants,
                        new PropertyCondition(property, type));
                    break;

                case LocatorStrategy.XPath:
                    element = this.uiAutomation.FindFirstByXPath(context.AutomationElement, locator);
                    break;

                default:
                    throw new FailedCommandException(
                        string.Format("Usupported locator startegy: {0}", strategy.ToString()),
                        32); // InvalidSelector (32)
            }

            return element == null ? null : this.elementFactory.GetElement(element);
        }

        public IList<IElement> FindAll(IElement context, LocatorStrategy strategy, string locator)
        {
            AutomationProperty property = this.ToAutomationProperty(strategy);
            IEnumerable elements = null;
            switch (strategy)
            {
                case LocatorStrategy.Id:
                case LocatorStrategy.Name:
                case LocatorStrategy.ClassName:
                    elements = context.AutomationElement.FindAll(
                        TreeScope.Descendants,
                        new PropertyCondition(property, locator));
                    break;

                case LocatorStrategy.TagName:
                    ControlType type = this.uiAutomation.FromTagName(locator);
                    elements = context.AutomationElement.FindAll(
                        TreeScope.Descendants,
                        new PropertyCondition(property, type));
                    break;

                case LocatorStrategy.XPath:
                    elements = this.uiAutomation.FindAllByXPath(context.AutomationElement, locator);
                    break;

                default:
                    throw new FailedCommandException(
                        string.Format("Usupported locator startegy: {0}", strategy.ToString()),
                        32); // InvalidSelector (32)
            }

            var results = new List<IElement>();
            foreach (var element in elements)
            {
                results.Add(this.elementFactory.GetElement((AutomationElement)element));
            }

            return results;
        }

        private AutomationProperty ToAutomationProperty(LocatorStrategy strategy)
        {
            switch (strategy)
            {
                case LocatorStrategy.Name:
                    return AutomationElement.NameProperty;

                case LocatorStrategy.TagName:
                    return AutomationElement.ControlTypeProperty;

                case LocatorStrategy.ClassName:
                    return AutomationElement.ClassNameProperty;

                default:
                    return AutomationElement.AutomationIdProperty;
            }
        }
    }
}