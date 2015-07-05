namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Windows.Automation;

    [Route("GET", "/session/:sessionId/element/:id/text")]
    internal class GetElementTextHandler : IHandler
    {
        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var element = session.GetUIElement(int.Parse(urlParams["id"]));

            object objPattern;
            if (element.TryGetCurrentPattern(TextPattern.Pattern, out objPattern))
            {
                return ((TextPattern)objPattern).DocumentRange.GetText(-1);
            }

            return element.Current.Name;
        }
    }
}