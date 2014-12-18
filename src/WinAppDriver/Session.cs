using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Automation;
using System.Text.RegularExpressions;

namespace WinAppDriver {

    class Session {
    
        private List<AutomationElement> uiElements;
    
        public Session(string id, Capabilities capabilities) {
            ID = id;
            Capabilities = capabilities;
            uiElements = new List<AutomationElement>();
        }

        public string ID { get; set; }
        
        public Capabilities Capabilities { get; set; }
        
        public int AddUIElement(AutomationElement element) {
            int id = uiElements.Count;
            uiElements.Add(element);
            Console.WriteLine("UI element ({0})) added.", id);
            return id;
        }

        public AutomationElement GetUIElement(int id) {
            return uiElements[id];
        }
        
    }

}

