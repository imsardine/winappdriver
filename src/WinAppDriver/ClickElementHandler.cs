using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Automation;

namespace WinAppDriver {

    [Route("POST", "/session/:sessionId/element/:id/click")]
    class ClickElementHandler : IHandler {

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session) {
            var element = session.GetUIElement(int.Parse(urlParams["id"]));
            ((InvokePattern)element.GetCurrentPattern(InvokePattern.Pattern)).Invoke();

            return null;
        }

    }

}

