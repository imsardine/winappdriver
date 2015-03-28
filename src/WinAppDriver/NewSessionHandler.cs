namespace WinAppDriver
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [Route("POST", "/session")]
    internal class NewSessionHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

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
                logger.Info("{0} = {1} ({2})", kvp.Key, kvp.Value, kvp.Value.GetType());
            }

            var caps = new Capabilities()
            {
                PlatformName = (string)request.DesiredCapabilities["platformName"],
                PackageName = (string)request.DesiredCapabilities["packageName"],
                App = request.DesiredCapabilities.ContainsKey("app") ? (string)request.DesiredCapabilities["app"] : null,
                MD5 = request.DesiredCapabilities.ContainsKey("MD5") ? (string)request.DesiredCapabilities["MD5"] : null
            };

            IStoreApp app = new StoreApp(caps.PackageName, this.utils);
            /**
            IPackageInstaller installer = new StoreAppPackageInstaller(app, this.utils, caps.App, caps.MD5);

            if (app.IsInstalled())
            {
                app.Terminate();
                if (caps.App != null)
                {
                    if (installer.IsBuildChanged())
                    {
                        app.Uninstall();
                        installer.Install();
                    }
                }
                else
                {
                    logger.Info("Store App is already installed and the capability of App is empty, so skip installing.");
                }
            }
            else
            {
                if (caps.App != null)
                {
                    installer.Install();
                }
                else
                {
                    string msg = "The source should be provided if Store App isn't installed.";
                    throw new WinAppDriverException(msg);
                }
            }

            app.Launch();
            **/
            session = this.sessionManager.CreateSession(app, caps);

            return caps; // TODO capabilities
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