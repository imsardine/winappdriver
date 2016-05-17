namespace WinAppDriver.Handlers
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using WinAppDriver.Desktop;
    using WinAppDriver.Modern;
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

        public object Handle(Dictionary<string, string> urlParams, string body, ref ISession session)
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
                    app = new StoreApp(this.context, caps, this.utils);
                    break;

                case Platform.WindowsPhone:
                    throw new FailedCommandException("Windows Phone is not supported yet.", 33);
                default:
                    throw new FailedCommandException("The platform name '{0}' is invalid.", 33);
            }

            // TODO validate capabilibites, add logs for identifying which decision path is chosen.
            if (app.IsInstalled())
            {
                if (caps.App != null)
                {
                    logger.Info(
                        "App '{0}' is already installed, and the installation package is also provided.",
                        app.DriverAppID);

                    if (app.Installer.IsBuildChanged())
                    {
                        logger.Info("Build changed. Strategy: {0}, Reset: {1}", caps.ChangeBuildStrategy, caps.ResetStrategy);

                        app.Terminate();
                        if (caps.ChangeBuildStrategy == ChangeBuildStrategy.Reinstall)
                        {
                            // reset strategy is irrelevant here
                            app.Uninstall();
                            app.Installer.Install();
                            app.BackupInitialStates(true);
                        }
                        else if (caps.ChangeBuildStrategy == ChangeBuildStrategy.Upgrade)
                        {
                            // full-reset strategy is irrelevant here
                            if (caps.ResetStrategy != ResetStrategy.No)
                            {
                                app.RestoreInitialStates();
                                app.Installer.Install();
                                app.BackupInitialStates(true);
                            }
                        }
                    }
                    else
                    {
                        logger.Info("Build not changed. Reset: {0}", caps.ResetStrategy);

                        if (caps.ResetStrategy == ResetStrategy.Fast)
                        {
                            app.Terminate();
                            app.RestoreInitialStates();
                        }
                        else if (caps.ResetStrategy == ResetStrategy.Full)
                        {
                            app.Terminate();
                            app.Uninstall();
                            app.Installer.Install();
                            app.BackupInitialStates(true);
                        }
                    }
                }
                else
                {
                    logger.Info(
                        "App '{0}' is already installed, but the installation package is not provided. Reset: {1}",
                        app.DriverAppID, caps.ResetStrategy);

                    // full-reset is irrelevant here
                    if (caps.ResetStrategy == ResetStrategy.Fast)
                    {
                        app.Terminate();
                        if (!app.BackupInitialStates(false))
                        {
                            app.RestoreInitialStates();
                        }
                    }
                }
            }
            else
            {
                if (caps.App != null)
                {
                    logger.Info(
                        "App '{0}' is not installed yet, but the installation package is provided.",
                        app.DriverAppID);

                    app.Installer.Install();
                    app.BackupInitialStates(true);
                }
                else
                {
                    logger.Info(
                        "App '{0}' is not installed yet, and the installation package is not provided. Reset: {1}",
                        app.DriverAppID, caps.ResetStrategy);

                    if (caps.Platform == Platform.Windows)
                    {
                        // full-reset is irrelevant here
                        if (caps.ResetStrategy == ResetStrategy.Fast)
                        {
                            app.Terminate();
                            app.RestoreInitialStates();
                        }
                    }
                    else
                    {
                        var msg = "'app' capability is mandatory if the target platform is not 'Windows' " +
                            "(default) and the app under test '{0}' is not installed beforehand.";
                        throw new WinAppDriverException(string.Format(msg, app.DriverAppID));
                    }
                }
            }

            if (caps.ResetStrategy != ResetStrategy.SkipActivate)
            {
            app.Activate();
            }

            session = this.sessionManager.CreateSession(app, caps);

            // TODO turn off IME, release all modifier keys
            return caps;
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