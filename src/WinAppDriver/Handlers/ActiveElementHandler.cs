namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/element/active")]
    internal class ActiveElementHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IUIAutomation uiAutomation;

        private IOverlay overlay;

        public ActiveElementHandler(IUIAutomation uiAutomation, IOverlay overlay)
        {
            this.uiAutomation = uiAutomation;
            this.overlay = overlay;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var element = this.uiAutomation.FocusedElement;

            this.overlay.Clear();
            this.overlay.HighlightedElement = element;
            this.overlay.Show();

            int id = session.AddUIElement(element.AutomationElement);
            return new Dictionary<string, string> { { "ELEMENT", id.ToString() } };
        }
    }
}