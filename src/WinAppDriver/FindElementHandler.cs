using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Automation;

namespace WinAppDriver {

    [Route("POST", "/session/:sessionId/element")]
    class FindElementHandler : IHandler {

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session) {
            FindElementRequest request = JsonConvert.DeserializeObject<FindElementRequest>(body);

            // TODO remove hard-coded windows title
            var desktop = AutomationElement.RootElement;
            var app = desktop.FindFirst(TreeScope.Element | TreeScope.Children,
                new PropertyCondition(AutomationElement.NameProperty, "KKBOX", PropertyConditionFlags.IgnoreCase));

            // TODO throw exceptions to indicate other strategies are not supported.
            var property = AutomationElement.AutomationIdProperty;
            if (request.Strategy == "name")
                property = AutomationElement.NameProperty;

            var element = app.FindFirst(TreeScope.Descendants, new PropertyCondition(
                property, request.Locator));
            if (element == null) {
                throw new NoSuchElementException(request.Strategy, request.Locator);
            }

            int id = session.AddUIElement(element);
            return new Dictionary<string, int> { { "ELEMENT", id } };
        }

        private class FindElementRequest {

            [JsonProperty("using")]
            internal string Strategy { get; set; }

            [JsonProperty("value")]
            internal string Locator { get; set; }

        }

    }

}

