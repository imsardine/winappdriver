namespace WinAppDriver
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Automation;

    internal class Session : ISession
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private List<AutomationElement> uiElements;

        public Session(string id, IApplication application, Capabilities capabilities)
        {
            this.ID = id;
            this.Application = application;
            this.Capabilities = capabilities;
            this.FocusOnCurrentWindow = true;
            this.ImplicitWaitMillis = 0;
            this.uiElements = new List<AutomationElement>();
        }

        public string ID { get; private set; }

        public IApplication Application { get; private set; }

        public Capabilities Capabilities { get; private set; }

        public bool FocusOnCurrentWindow { get; set; }

        public int ImplicitWaitMillis { get; set; }

        public int AddUIElement(AutomationElement element)
        {
            int id = this.uiElements.Count;
            this.uiElements.Add(element);
            logger.Debug("UI element ({0})) added.", id);
            return id;
        }

        public AutomationElement GetUIElement(int id)
        {
            return this.uiElements[id];
        }
    }
}