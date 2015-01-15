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
                PlatformName = (string)request.DesiredCapabilities["platformName"],
                AppUserModelId = (string)request.DesiredCapabilities["appUserModelId"],
                App = (string)request.DesiredCapabilities["app"],
                MD5 = request.DesiredCapabilities.ContainsKey("MD5") ? (string)request.DesiredCapabilities["MD5"] : null
            };

            IStoreApplication app = new StoreApplication(caps.AppUserModelId, this.utils);

            if (caps.MD5 != null && caps.MD5 == app.GetLocalMD5())
            {
                Console.Out.WriteLine("\nThe current installed version and the assigned version are the same ,so skip installing.\n");
            }
            else
            {
                if (caps.App.EndsWith(".zip"))
                {
                    if (caps.App.StartsWith("http"))
                    {
                        caps.App = app.GetAppFileFromWeb(caps.App, caps.MD5);
                    }

                    Console.WriteLine("\nApp file:\n\t" + caps.App);

                    if (app.GetLocalMD5() == app.GetFileMD5(caps.App))
                    {
                        Console.Out.WriteLine("\nThe current installed version and the download version are the same ,so skip installing.\n");
                    }
                    else
                    {
                        if (app.IsInstalled())
                        {
                            app.UninstallApp(app.GetPackageFullName());
                        }

                        app.InstallApp(caps.App);
                    }
                }
                else
                {
                    throw new FailedCommandException("Your app file is \"" + caps.App + "\". App file is not a .zip file.", 13);
                }
            }

            app.Terminate();
            app.Activate();
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
