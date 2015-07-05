namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using WinAppDriver.UI;

    [Route("GET", "/session/:sessionId/title")]
    internal class GetTitleHandler : IHandler
    {
        private IUIAutomation uiAutomation;

        public GetTitleHandler(IUIAutomation uiAutomation)
        {
            this.uiAutomation = uiAutomation;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            return this.uiAutomation.GetFocusedWindowOrRoot().Current.Name;
        }
    }
}