namespace WinAppDriver
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [Route("POST", "/session")]
    internal class NewSessionHandler : IHandler
    {
        private SessionManager sessionManager;

        private IUtils utils;

        public NewSessionHandler(SessionManager sessionManager, IUtils utils)
        {
            this.sessionManager = sessionManager;
            this.utils = utils;
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

            IStoreApplication app = new StoreApplication(caps.AppUserModelId, this.utils);
            app.BackupInitialStates(); // TODO only when newly installed
            app.Activate();
            session = this.sessionManager.CreateSession(app, caps);

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