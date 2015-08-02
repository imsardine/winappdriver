namespace WinAppDriver.Handlers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
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

        private IElementSearcher searcher;

        public FindElementsHandler(
            IUIAutomation uiAutomation, IOverlay overlay, IElementFactory elementFactory,
            IElementSearcher searcher)
        {
            this.uiAutomation = uiAutomation;
            this.overlay = overlay;
            this.elementFactory = elementFactory;
            this.searcher = searcher;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var request = JsonConvert.DeserializeObject<FindElementsRequest>(body);

            IElement context = null;
            if (urlParams.ContainsKey("id"))
            {
                context = this.elementFactory.GetElement(
                    session.GetUIElement(int.Parse(urlParams["id"])));
            }
            else
            {
                context = this.elementFactory.GetElement(
                    session.FocusOnCurrentWindow ?
                    this.uiAutomation.GetFocusedWindowOrRoot() :
                    AutomationElement.RootElement);
            }

            IList<IElement> elements = null;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            do
            {
                elements = this.searcher.FindAll(context, request.Strategy, request.Locator);
            }
            while (elements.Count == 0 && stopwatch.ElapsedMilliseconds < session.ImplicitWaitMillis);

            this.overlay.Clear();
            this.overlay.ContextElement = context;
            this.overlay.HighlightedElements = elements;
            this.overlay.Show();

            var list = new List<Dictionary<string, string>>();
            foreach (IElement element in elements)
            {
                int id = session.AddUIElement(element.AutomationElement);
                list.Add(new Dictionary<string, string> { { "ELEMENT", id.ToString() } });
            }

            return list;
        }

        private class FindElementsRequest
        {
            [JsonProperty("using")]
            internal LocatorStrategy Strategy { get; set; }

            [JsonProperty("value")]
            internal string Locator { get; set; }
        }
    }
}