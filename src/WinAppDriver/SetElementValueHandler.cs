using System.Text;
using System.Collections.Generic;
using System.Windows.Automation;
using Newtonsoft.Json;

namespace WinAppDriver {

    [Route("POST", "/session/:sessionId/element/:id/value")]
    class SetElementValueHandler : IHandler {

        public object Handle(Dictionary<string, string> urlParams, string body, Session session) {
            var element = session.GetUIElement(int.Parse(urlParams["id"]));
            var request = JsonConvert.DeserializeObject<ElementValueRequest>(body);
            
            var sb = new StringBuilder();
            foreach (var key in request.KeySequence)
                sb.Append(key);
            var value = sb.ToString();
            
            ((ValuePattern)element.GetCurrentPattern(ValuePattern.Pattern)).SetValue(value);

            return new Dictionary<string, object> {
                { "sessionId", session.ID },
                { "status", 0 },
                { "value",  null }
            };        
        }

        private class ElementValueRequest {

            [JsonProperty("value")]
            internal string[] KeySequence { get; set; }

        }

    }

}

