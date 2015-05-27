namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Windows.Automation;

    [Route("GET", "/session/:sessionId/window_handles")]
    internal class GetWindowsHandler : IHandler
    {
        private IUIAutomation uiAutomation;

        public GetWindowsHandler(IUIAutomation uiAutomation)
        {
            this.uiAutomation = uiAutomation;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            return this.uiAutomation.GetTopLevelWindowHandles();
        }
    }
}