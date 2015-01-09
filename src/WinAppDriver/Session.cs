namespace WinAppDriver
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Automation;

    internal class Session
    {
        private List<AutomationElement> uiElements;

        public Session(string id, IApplication application, Capabilities capabilities)
        {
            this.ID = id;
            this.Application = application;
            this.Capabilities = capabilities;
            this.uiElements = new List<AutomationElement>();
        }

        public string ID { get; private set; }

        public IApplication Application { get; private set; }

        public Capabilities Capabilities { get; private set; }

        public int AddUIElement(AutomationElement element)
        {
            int id = this.uiElements.Count;
            this.uiElements.Add(element);
            Console.WriteLine("UI element ({0})) added.", id);
            return id;
        }

        public AutomationElement GetUIElement(int id)
        {
            return this.uiElements[id];
        }
    }
}