namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Windows.Automation;
    using WinAppDriver.UI;

    [Route("GET", "/session/:sessionId/source")]
    internal class GetSourceHandler : IHandler
    {
        private IUIAutomation uiAutomation;

        public GetSourceHandler(IUIAutomation uiAutomation)
        {
            this.uiAutomation = uiAutomation;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var start = session.FocusOnCurrentWindow ?
                this.uiAutomation.GetFocusedWindowOrRoot() :
                AutomationElement.RootElement;
            return this.uiAutomation.DumpXml(start);
        }
    }
}