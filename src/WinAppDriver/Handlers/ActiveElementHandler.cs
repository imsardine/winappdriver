namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using WinAppDriver.UI;

    [Route("POST", "/session/:sessionId/element/active")]
    internal class ActiveElementHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private IUIAutomation uiAutomation;

        public ActiveElementHandler(IUIAutomation uiAutomation)
        {
            this.uiAutomation = uiAutomation;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            int id = session.AddUIElement(this.uiAutomation.FocusedElement.AutomationElement);
            return new Dictionary<string, string> { { "ELEMENT", id.ToString() } };
        }
    }
}