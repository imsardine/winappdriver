namespace WinAppDriver
{
    using System.Collections.Generic;
    using System.Windows.Automation;
    using Newtonsoft.Json;

    [Route("POST", "/session/:sessionId/element")]
    [Route("POST", "/session/:sessionId/element/:id/element")]
    internal class FindElementHandler : IHandler
    {
        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            FindElementRequest request = JsonConvert.DeserializeObject<FindElementRequest>(body);

            var root = AutomationElement.RootElement;
            if (urlParams.ContainsKey("id"))
            {
                root = session.GetUIElement(int.Parse(urlParams["id"]));
            }

            // TODO throw exceptions to indicate other strategies are not supported.
            var property = AutomationElement.AutomationIdProperty;
            if (request.Strategy == "name")
            {
                property = AutomationElement.NameProperty;
            }

            var element = root.FindFirst(
                TreeScope.Descendants,
                new PropertyCondition(property, request.Locator));
            if (element == null)
            {
                new KeyboardImpl().ShowCharmsMenu();
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