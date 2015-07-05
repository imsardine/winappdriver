namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Automation;
    using Newtonsoft.Json;

    [Route("POST", "/session/:sessionId/element/:id/value")]
    internal class SetElementValueHandler : IHandler
    {
        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
        {
            var element = session.GetUIElement(int.Parse(urlParams["id"]));
            var request = JsonConvert.DeserializeObject<ElementValueRequest>(body);

            var sb = new StringBuilder();
            foreach (var key in request.KeySequence)
            {
                sb.Append(key);
            }

            var value = sb.ToString();
            ((ValuePattern)element.GetCurrentPattern(ValuePattern.Pattern)).SetValue(value);

            return null;
        }

        private class ElementValueRequest
        {
            [JsonProperty("value")]
            internal string[] KeySequence { get; set; }
        }
    }
}