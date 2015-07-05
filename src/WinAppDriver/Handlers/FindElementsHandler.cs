namespace WinAppDriver.Handlers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Automation;
    using Newtonsoft.Json;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/elements")]
    [Route("POST", "/session/:sessionId/element/:id/elements")]
    internal class FindElementsHandler : IHandler
    {
        private IUIAutomation uiAutomation;

        public FindElementsHandler(IUIAutomation uiAutomation)
        {
            this.uiAutomation = uiAutomation;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            FindElementRequest request = JsonConvert.DeserializeObject<FindElementRequest>(body);

            AutomationElement start = null;
            if (urlParams.ContainsKey("id"))
            {
                start = session.GetUIElement(int.Parse(urlParams["id"]));
            }
            else
            {
                start = session.FocusOnCurrentWindow ? 
                    this.uiAutomation.GetFocusedWindowOrRoot() :
                    AutomationElement.RootElement;
            }

            IEnumerable elements = null;
            if (request.Strategy == "xpath")
            {
                elements = this.uiAutomation.FindAllByXPath(start, request.Locator);
            }
            else
            {
                var property = AutomationElement.AutomationIdProperty;
                if (request.Strategy == "name")
                {
                    property = AutomationElement.NameProperty;
                }
                else if (request.Strategy == "class name")
                {
                    property = AutomationElement.ClassNameProperty;
                }

                elements = start.FindAll(
                    TreeScope.Descendants,
                    new PropertyCondition(property, request.Locator));
            }

            var list = new List<Dictionary<string, string>>();
            foreach (AutomationElement element in elements)
            {
                int id = session.AddUIElement(element);
                list.Add(new Dictionary<string, string> { { "ELEMENT", id.ToString() } });
            }

            return list;
        }

        private class FindElementRequest
        {
            [JsonProperty("using")]
            internal string Strategy { get; set; }

            [JsonProperty("value")]
            internal string Locator { get; set; }
        }
    }
}