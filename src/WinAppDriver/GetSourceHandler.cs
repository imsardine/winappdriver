namespace WinAppDriver
{
    using System.Collections.Generic;
    using System.Windows.Automation;

    [Route("GET", "/session/:sessionId/source")]
    internal class GetSourceHandler : IHandler
    {
        private IUIAutomation uiAutomation;

        public GetSourceHandler(IUIAutomation uiAutomation)
        {
            this.uiAutomation = uiAutomation;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            return uiAutomation.DumpXml(AutomationElement.RootElement);
        }
    }
}