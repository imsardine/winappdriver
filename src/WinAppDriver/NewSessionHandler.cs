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

        private IDriverContext context;

        private SessionManager sessionManager;

        private IUACPomptHandler uacHandler;

        private IUtils utils;

        public NewSessionHandler(IDriverContext context, SessionManager sessionManager, IUACPomptHandler uacHandler, IUtils utils)
        {
            this.context = context;
            this.sessionManager = sessionManager;
            this.uacHandler = uacHandler;
            this.utils = utils;
        }

        public object Handle(Dictionary<string, string> urlParams, string body, ref Session session)
        {
            var request = JsonConvert.DeserializeObject<NewSessionRequestAsDict>(body);
            foreach (var kvp in request.DesiredCapabilities)
            {
                logger.Info("{0} = {1} ({2})", kvp.Key, kvp.Value, kvp.Value.GetType());
            }

            var caps = JsonConvert.DeserializeObject<NewSessionRequest>(body).DesiredCapabilities;

            IApplication app;
            switch (caps.Platform)
            {
                case Platform.Windows:
                    app = new DesktopApp(this.context, caps, this.uacHandler, this.utils);
                    break;

                case Platform.WindowsModern:
                    app = new StoreApp(caps, this.utils);
                    break;

                case Platform.WindowsPhone:
                    throw new FailedCommandException("Windows Phone is not supported yet.", 33);
                default:
                    throw new FailedCommandException("The platform name '{0}' is invalid.", 33);
            }

            // TODO validate capabilibites
            if (app.IsInstalled())
            {
                if (caps.App != null)
                {
                    if (app.Installer.IsBuildChanged())
                    {
                        app.Terminate();
                        app.Uninstall(); // TODO if strategy == reinstall
                        app.Installer.Install();
                        app.BackupInitialStates();
                    }
                    else
                    {
                        // TODO fast/full/no reset
                    }
                }
                else
                {
                    // TODO fast/no reset (full reset is irrelevant here)
                    logger.Info("Store App is already installed and the capability of App is empty, so skip installing.");
                }
            }
            else
            {
                if (caps.App != null)
                {
                    app.Installer.Install();
                    app.BackupInitialStates();
                }
                else
                {
                    if (caps.Platform != Platform.Windows)
                    {
                        var msg = "'app' capability is mandatory if the target platform is not 'Windows' " +
                            "(default) and the app under test '{0}' is not installed beforehand.";
                        throw new WinAppDriverException(string.Format(msg, app.DriverAppID));
                    }
                }
            }

            app.Activate();
            session = this.sessionManager.CreateSession(app, caps);

            // TODO turn off IME, release all modifier keys
            return caps; // TODO capabilities
        }

        private class NewSessionRequest
        {
            [JsonProperty("desiredCapabilities")]
            internal Capabilities DesiredCapabilities { get; set; }

            [JsonProperty("requiredCapabilities")]
            internal Capabilities RequiredCapabilities { get; set; }
        }

        private class NewSessionRequestAsDict
        {
            [JsonProperty("desiredCapabilities")]
            internal Dictionary<string, object> DesiredCapabilities { get; set; }

            [JsonProperty("requiredCapabilities")]
            internal Dictionary<string, object> RequiredCapabilities { get; set; }
        }
    }
}