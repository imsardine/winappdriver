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
                PlatformName = (string)request.DesiredCapabilities["platformName"],
                PackageName = (string)request.DesiredCapabilities["packageName"],
                App = request.DesiredCapabilities.ContainsKey("app") ? (string)request.DesiredCapabilities["app"] : null,
                MD5 = request.DesiredCapabilities.ContainsKey("MD5") ? (string)request.DesiredCapabilities["MD5"] : null
            };

            IStoreApplication app = new StoreApplication(caps.PackageName, this.utils);

            if (app.IsInstalled())
            {
                app.Terminate();
                if (caps.App != null)
                {
                    if (caps.App.EndsWith(".zip"))
                    {
                        string downloadedFile = caps.App;
                        string downloadedFileMD5 = null;
                        string localMd5 = app.GetLocalMD5();
                        if (caps.MD5 != null)
                        {
                            if (localMd5 != caps.MD5)
                            {
                                downloadedFile = this.utils.GetAppFileFromWeb(caps.App, caps.MD5);
                                downloadedFileMD5 = this.utils.GetFileMD5(downloadedFile);
                                app.Uninstall();
                                app.Install(downloadedFile);
                            }
                            else
                            {
                                Console.Out.WriteLine("\nThe current installed version and the assigned version are the same ,so skip installing.\n");
                            }
                        }
                        else
                        {
                            downloadedFile = this.utils.GetAppFileFromWeb(caps.App, caps.MD5);
                            downloadedFileMD5 = this.utils.GetFileMD5(downloadedFile);
                            if (localMd5 != downloadedFileMD5)
                            {
                                app.Uninstall();
                                app.Install(downloadedFile);
                            }
                            else
                            {
                                Console.Out.WriteLine("\nThe current installed version and the download version are the same ,so skip installing.\n");
                            }
                        }
                    }
                    else
                    {
                        throw new FailedCommandException("Your app file is \"" + caps.App + "\". App file is not a .zip file.", 13);
                    }
                }
            }
            else
            {
                if (caps.App != null)
                {
                    if (caps.App.EndsWith(".zip"))
                    {
                        string downloadedFile = this.utils.GetAppFileFromWeb(caps.App, caps.MD5);
                        app.Install(downloadedFile);
                    }
                    else
                    {
                        throw new FailedCommandException("Your app file is \"" + caps.App + "\". App file is not a .zip file.", 13);
                    }
                }
                else
                {
                    string msg = "There is no installed App neither install file.";
                    throw new WinAppDriverException(msg);
                }
            }

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