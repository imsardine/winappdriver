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

        private IOverlay overlay;

        private IElementFactory elementFactory;

        public FindElementsHandler(IUIAutomation uiAutomation, IOverlay overlay, IElementFactory elementFactory)
        {
            this.uiAutomation = uiAutomation;
            this.overlay = overlay;
            this.elementFactory = elementFactory;
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
                object locator = request.Locator;

                if (request.Strategy == "name")
                {
                    property = AutomationElement.NameProperty;
                }
                else if (request.Strategy == "class name")
                {
                    property = AutomationElement.ClassNameProperty;
                }
                else if (request.Strategy == "id")
                {
                    property = AutomationElement.AutomationIdProperty;
                }
                else if (request.Strategy == "tag name")
                {
                    property = AutomationElement.ControlTypeProperty;
                    locator = this.uiAutomation.FromTagName(request.Locator);
                }

                elements = start.FindAll(
                    TreeScope.Descendants,
                    new PropertyCondition(property, locator));
            }

            var highlighted = new List<IElement>();
            var list = new List<Dictionary<string, string>>();
            foreach (AutomationElement element in elements)
            {
                highlighted.Add(this.elementFactory.GetElement(element));
                int id = session.AddUIElement(element);
                list.Add(new Dictionary<string, string> { { "ELEMENT", id.ToString() } });
            }

            this.overlay.Clear();
            this.overlay.ContextElement = this.elementFactory.GetElement(start);
            this.overlay.HighlightedElements = highlighted;
            this.overlay.Show();

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