namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Windows.Automation;

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
            var handle = (int)this.uiAutomation.GetFocusedWindowOrRoot().GetCurrentPropertyValue(
                AutomationElement.NativeWindowHandleProperty, true);
            return handle.ToString();
        }
    }
}