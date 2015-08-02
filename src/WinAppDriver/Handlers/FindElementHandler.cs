namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Automation;
    using Newtonsoft.Json;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/element")]
    [Route("POST", "/session/:sessionId/element/:id/element")]
    internal class FindElementHandler : IHandler
    {
        private IUIAutomation uiAutomation;

        private IOverlay overlay;

        private IElementFactory elementFactory;

        private IElementSearcher searcher;

        public FindElementHandler(
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
            FindElementRequest request = JsonConvert.DeserializeObject<FindElementRequest>(body);

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

            IElement element = null;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            do
            {
                element = this.searcher.FindFirst(context, request.Strategy, request.Locator);
            } 
            while (element == null && stopwatch.ElapsedMilliseconds < session.ImplicitWaitMillis);

            this.overlay.Clear();
            this.overlay.ContextElement = context;
            this.overlay.HighlightedElement = element;
            this.overlay.Show();

            if (element == null)
            {
                throw new NoSuchElementException(request.Strategy.ToString(), request.Locator);
            }

            int id = session.AddUIElement(element.AutomationElement);
            return new Dictionary<string, string> { { "ELEMENT", id.ToString() } };
        }

        private class FindElementRequest
        {
            [JsonProperty("using")]
            internal LocatorStrategy Strategy { get; set; }

            [JsonProperty("value")]
            internal string Locator { get; set; }
        }
    }
}