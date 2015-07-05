namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Windows.Automation;
    using Newtonsoft.Json;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/element")]
    [Route("POST", "/session/:sessionId/element/:id/element")]
    internal class FindElementHandler : IHandler
    {
        private IUIAutomation uiAutomation;

        public FindElementHandler(IUIAutomation uiAutomation)
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

            AutomationElement element = null;
            if (request.Strategy == "xpath")
            {
                element = this.uiAutomation.FindFirstByXPath(start, request.Locator);
            }
            else
            {
                // TODO throw exceptions to indicate other strategies are not supported.
                var property = AutomationElement.AutomationIdProperty;
                if (request.Strategy == "name")
                {
                    property = AutomationElement.NameProperty;
                }
                else if (request.Strategy == "class name")
                {
                    property = AutomationElement.ClassNameProperty;
                }

                element = start.FindFirst(
                    TreeScope.Descendants,
                    new PropertyCondition(property, request.Locator));
            }

            if (element == null)
            {
                throw new NoSuchElementException(request.Strategy, request.Locator);
            }

            int id = session.AddUIElement(element);
            return new Dictionary<string, string> { { "ELEMENT", id.ToString() } };
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