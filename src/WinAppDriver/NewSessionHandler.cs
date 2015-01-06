using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace WinAppDriver {

    [Route("POST", "/session")]
    class NewSessionHandler : IHandler {

        private SessionManager sessionManager;

        public NewSessionHandler(SessionManager sessionManager) {
            this.sessionManager = sessionManager;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session) {
            NewSessionRequest request = JsonConvert.DeserializeObject<NewSessionRequest>(body);
            foreach (var kvp in request.DesiredCapabilities)
                Console.WriteLine("{0} = {1} ({2})", kvp.Key, kvp.Value, kvp.Value.GetType());

            var caps = new Capabilities() {
                AppUserModelId = (string)request.DesiredCapabilities["appUserModelId"],
                App = (string)request.DesiredCapabilities["app"]
            };

            string localAppDataLocation = Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%");
            string packageFamilyName = caps.AppUserModelId.Split(new char[] { '!' })[0];
            string originSettings = localAppDataLocation + "\\Packages\\" + packageFamilyName + @"\Settings\";
            string destSettings = localAppDataLocation + "\\WinAppDriver\\InitialStates\\" + packageFamilyName + @"\Settings\";
            string originLocalState = localAppDataLocation + "\\Packages\\" + packageFamilyName + @"\LocalState\";
            string destLocalState = localAppDataLocation + "\\WinAppDriver\\InitialStates\\" + packageFamilyName + @"\LocalState\";
  
            if (!Directory.Exists(destSettings))
            {
                DirectoryCopyHelper.Copy(originSettings, destSettings, true, true);
            }
            if (!Directory.Exists(destLocalState))
            {
                DirectoryCopyHelper.Copy(originLocalState, destLocalState, true, true);
            }

            Process.Start("ActivateStoreApp", caps.AppUserModelId);
            session = sessionManager.CreateSession(caps);

            return null; // TODO capabilities
        }

        private class NewSessionRequest {

            [JsonProperty("desiredCapabilities")]
            internal Dictionary<string, object> DesiredCapabilities { get; set; }

            [JsonProperty("requiredCapabilities")]
            internal Dictionary<string, object> RequiredCapabilities { get; set; }

        }

    }

}

