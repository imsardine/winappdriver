namespace WinAppDriver
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using WinAppDriver.Desktop;
    using WinAppDriver.UAC;

    [Route("POST", "/session")]
    internal class NewSessionHandler : IHandler
    {
        private static ILogger logger = Logger.GetLogger("WinAppDriver");

        private SessionManager sessionManager;

        private IUACPomptHandler uacHandler;

        private IUtils utils;

        public NewSessionHandler(SessionManager sessionManager, IUACPomptHandler uacHandler, IUtils utils)
        {
            this.sessionManager = sessionManager;
            this.uacHandler = uacHandler;
            this.utils = utils;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            NewSessionRequest request = JsonConvert.DeserializeObject<NewSessionRequest>(body);
            foreach (var kvp in request.DesiredCapabilities)
            {
                logger.Info("{0} = {1} ({2})", kvp.Key, kvp.Value, kvp.Value.GetType());
            }

            var caps = new Capabilities(request.DesiredCapabilities);
            if (caps.PlatformName == null)
            {
                throw new FailedCommandException("The 'platformName' desired capability is mandatory in any cases. It can be 'Windows', 'WindowsModern' or 'WindowsPhone'.", 33); // TODO define enumeration
            }

            // valid platform names: Windows, WindowModern, WindowPhone
            IApplication app;
            switch (caps.PlatformName)
            {
                case "Windows":
                    app = new DesktopApp(caps, this.uacHandler, this.utils);
                    break;

                case "WindowsModern":
                    app = new StoreApp(caps, this.utils);
                    break;

                case "WindowsPhone":
                    throw new FailedCommandException("Windows Phone is not supported yet.", 33);
                default:
                    throw new FailedCommandException("The platform name '{0}' is invalid.", 33);
            }

            if (app.IsInstalled())
            {
                app.Terminate();
                if (caps.App != null)
                {
                    if (app.Installer.IsBuildChanged())
                    {
                        app.Uninstall();
                        app.Installer.Install();
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
                    app.Installer.Install();
                }
                else
                {
                    string msg = "The source should be provided if Store App isn't installed.";
                    throw new WinAppDriverException(msg);
                }
            }

            app.Launch();
            session = this.sessionManager.CreateSession(app, caps);

            // TODO turn off IME, release all modifier keys
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