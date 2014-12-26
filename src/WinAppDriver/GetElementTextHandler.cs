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
            return element.Current.Name.ToString();
        }
    }
}
