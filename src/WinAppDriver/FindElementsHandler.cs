using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using Newtonsoft.Json;


namespace WinAppDriver
{
    [Route("POST", "/session/:sessionId/elements")]
    [Route("POST", "/session/:sessionId/element/:id/elements")]
    class FindElementsHandler : IHandler {

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session) {
            FindElementRequest request = JsonConvert.DeserializeObject<FindElementRequest>(body);

            var root = AutomationElement.RootElement;
            if(urlParams.ContainsKey("id"))
            {
                root = session.GetUIElement(int.Parse(urlParams["id"]));
            }

            var property = AutomationElement.AutomationIdProperty;
            if (request.Strategy == "name")
                property = AutomationElement.NameProperty;

            var elements = root.FindAll(TreeScope.Descendants, new PropertyCondition(
                property, request.locator));
  
            var list = new List<Dictionary<string, string>>();
            foreach(AutomationElement element in elements) {
                int id = session.AddUIElement(element);
                list.Add(new Dictionary<string, string> { {"ELEMENT", id.ToString()} });
            }
            return list;
        }

        private class FindElementRequest {
            [JsonProperty("using")]
            internal string Strategy { get; set; }

            [JsonProperty("value")]
            internal string locator { get; set; }
        }
    }
}
