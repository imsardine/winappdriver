namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;

    [Route("GET", "/session/:sessionId/window_handle")]
    internal class GetCurrentWindowHandler : IHandler
    {
        private IUIAutomation uiAutomation;

        public GetCurrentWindowHandler(IUIAutomation uiAutomation)
        {
            this.uiAutomation = uiAutomation;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            var window = this.uiAutomation.GetFocusedWindowOrRoot();
            return this.uiAutomation.ToNativeWindowHandle(window).ToString();
        }
    }
}