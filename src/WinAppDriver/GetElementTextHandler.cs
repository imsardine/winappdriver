using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Automation;

namespace WinAppDriver
{

    [Route("GET", "/session/:sessionId/element/:id/text")]
    class GetElementTextHandler : IHandler
    {

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            var element = session.GetUIElement(int.Parse(urlParams["id"]));
            if (element.GetCurrentPropertyValue(AutomationElement.IsTextPatternAvailableProperty).Equals(true))
            {
                return ((TextPattern)element.GetCurrentPattern(TextPattern.Pattern)).DocumentRange.GetText(-1);
            }
            return element.Current.Name;
        }
    }
}
