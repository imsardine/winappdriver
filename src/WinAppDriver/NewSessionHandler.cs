namespace WinAppDriver
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Newtonsoft.Json;

    [Route("POST", "/session")]
    internal class NewSessionHandler : IHandler
    {
        private SessionManager sessionManager;

        public NewSessionHandler(SessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            NewSessionRequest request = JsonConvert.DeserializeObject<NewSessionRequest>(body);
            foreach (var kvp in request.DesiredCapabilities)
            {
                Console.WriteLine("{0} = {1} ({2})", kvp.Key, kvp.Value, kvp.Value.GetType());
            }

            var caps = new Capabilities()
            {
                AppUserModelId = (string)request.DesiredCapabilities["appUserModelId"],
                App = (string)request.DesiredCapabilities["app"]
            };

            Process.Start("ActivateStoreApp", caps.AppUserModelId);
            session = this.sessionManager.CreateSession(caps);

            return null; // TODO capabilities
        }

        private class NewSessionRequest
        {
            [JsonProperty("desiredCapabilities")]
            internal Dictionary<string, object> DesiredCapabilities { get; set; }

            [JsonProperty("requiredCapabilities")]
            internal Dictionary<string, object> RequiredCapabilities { get; set; }
        }
    }
}