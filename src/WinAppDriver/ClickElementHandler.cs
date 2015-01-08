namespace WinAppDriver
{
    using System.Collections.Generic;
    using System.Windows.Automation;

    [Route("POST", "/session/:sessionId/element/:id/click")]
    internal class ClickElementHandler : IHandler
    {
        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            var element = session.GetUIElement(int.Parse(urlParams["id"]));
            ((InvokePattern)element.GetCurrentPattern(InvokePattern.Pattern)).Invoke();

            return null;
        }
    }
}