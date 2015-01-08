namespace WinAppDriver
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Automation;

    internal class Session
    {
        private List<AutomationElement> uiElements;

        public Session(string id, Capabilities capabilities)
        {
            this.ID = id;
            this.Capabilities = capabilities;
            this.uiElements = new List<AutomationElement>();
        }

        public string ID { get; set; }

        public Capabilities Capabilities { get; set; }

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