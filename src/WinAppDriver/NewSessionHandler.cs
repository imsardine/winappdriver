using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

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

            if (caps.App.StartsWith("http"))
            {
                caps.App = GetAppFileFromWeb(caps.App);
            }

            Process.Start("ActivateStoreApp", caps.AppUserModelId);
            session = sessionManager.CreateSession(caps);

            // TODO formal capabilities
            return new Dictionary<string, string>
            {
                { "platformName", "WinApp" }
            };
        }

        private class NewSessionRequest {

            [JsonProperty("desiredCapabilities")]
            internal Dictionary<string, object> DesiredCapabilities { get; set; }

            [JsonProperty("requiredCapabilities")]
            internal Dictionary<string, object> RequiredCapabilities { get; set; }

        }

        private string GetAppFileFromWeb(string webResource)
        {
            string storeFileName = Environment.GetEnvironmentVariable("TEMP") + @"\StoreApp_" + DateTime.Now.ToString("yyyyMMddHHmmss") + webResource.Substring(webResource.LastIndexOf("."));
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();

            Console.WriteLine("Downloading File \"{0}\" .......\n\n", webResource);
            // Download the Web resource and save it into temp folder.
            myWebClient.DownloadFile(webResource, storeFileName);
            Console.WriteLine("Successfully Downloaded File \"{0}\"", webResource);
            Console.WriteLine("\nDownloaded file saved in the following file system folder:\n\t" + storeFileName);
            return storeFileName;
        }
    }

}

